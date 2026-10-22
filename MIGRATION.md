# Grid System v2 — migration handoff for the local agent

You continue the migration of the Unity plugin "Grid System 2D/3D by at0m" from the old implementation (`Assets/GridSystem/Examples`, Zenject) to v2 (`Assets/GridSystem/New`).
Read this file, `README.md` and the code before doing anything. Reply to the user in Russian, briefly, without apologies.

## 0. Ground rules from the user (mandatory)

- Confirm the task and the plan before large changes. When the user asks "why", only explain; do not change code.
- Never change scenes or prefabs without explicit permission. Finish the code, then ask, describing the needed setup.
- Do not commit or push until the user has verified the functionality and allowed it. Branch: `claude/pensive-davinci-1nnmef`. No PRs unless asked.
- Use the Unity MCP when the editor is open. Edit YAML (`.unity`, `.prefab`, `.asset`) only when MCP cannot do it.
- No generators or editor tools that build content, unless explicitly requested.
- Reuse old assets where possible: move them with their `.meta` to keep GUIDs, then adapt them. Do not recreate what exists.
- Code style:
  - no `var`;
  - braces always, on a new line;
  - expression bodies for one-line methods;
  - `<summary>` on public members;
  - no comments, only summaries;
  - no early return or continue, all logic inside `if`;
  - magic numbers and strings in constants;
  - no defensive checks of serialized fields (errors must be visible), validate only loaded data;
  - no logs unless asked (then `Debug.LogError`, keep them until the user confirms the fix);
  - enum checks go to a serialized field;
  - minimal allocations;
  - `FindObjectOfType`, `AddComponent`, `GetComponentsIn*` are forbidden (`GetComponent` only on raycast or collider hits);
  - namespaces follow folders.
- Priorities: 1) architecture, 2) performance and allocations, 3) flexibility.
- Decoupling goes through MessagePipe messages, not direct references or extra constructor parameters.

## 1. Repository state

- Unity `6000.3.6f1`, Built-in RP. Installed: Reflex 14.4, Extenject, VContainer 1.19, MessagePipe (+ Zenject/VContainer adapters), UniTask, R3 (NuGet), PrimeTween, Input System, TextMeshPro (Essentials imported), Addressables.
- The branch was reset to the state before the sample generator (commit `32361ec`), plus:
  - updated `Packages/manifest.json` and `packages-lock.json`;
  - `Assets/TextMesh Pro`;
  - `Assets/Resources/ReflexSettings.asset`, required by Reflex;
  - `.meta` files.
- `Assets/GridSystem/Examples` — the old implementation and its scene `Examples/Scenes/TestInventoryScene.unity`. It is the reference to migrate and will be deleted later.
- `Assets/GridSystem/Core` (`Grid`, `ColumnList`, `CellType`) — old core, used only by `Examples`.
- `Assets/GridSystem/New` — v2. Not verified in Play Mode yet. Only compilation was checked, outside Unity.

## 2. Packages and assemblies (`Assets/GridSystem/New`)

| Folder | Package | Assembly | References |
|---|---|---|---|
| `GridSystem/Core` | `com.at0m.gridsystem` | `GridSystem.Core` | — (pure C#, no Unity objects except math types) |
| `GridSystem/Runtime` | same | `GridSystem.Runtime` | Core, MessagePipe, UniTask, Unity.InputSystem, UnityEngine.UI, PrimeTween.Runtime (versionDefine `GRID_SYSTEM_PRIMETWEEN`) |
| `GridSystem/Editor` | same | `GridSystem.Editor` | Core, Runtime (shape drawer: LMB toggles cell, RMB sets pivot; `Validation/`: report types, `GridValidatedEditor` base inspector, `GridContentValidator` checks, settings inspector) |
| `Integrations/Zenject` | `com.at0m.gridsystem.zenject` | `GridSystem.Integrations.Zenject` | Core, Runtime, Zenject |
| `Integrations/Reflex` | `com.at0m.gridsystem.reflex` | `GridSystem.Integrations.Reflex` | Core, Runtime, Reflex |
| `Integrations/VContainer` | `com.at0m.gridsystem.vcontainer` | `GridSystem.Integrations.VContainer` | Core, Runtime, VContainer |
| `Samples` | not packaged | `GridSystem.Samples` (autoReferenced false) | Core, Runtime, MessagePipe, UniTask, Input System, UGUI, TMP |
| `Samples/Editor` | not packaged | `GridSystem.Samples.Editor` | Core, Runtime, Editor, Samples, Input System (sample config inspector) |

A project installs the core plus exactly one integration. `DOTWEEN` define enables `DOTweenItemMotion`. MessagePipe for Unity exposes UniTask types, so every assembly using MessagePipe must reference `UniTask` (CS0012 otherwise).

## 3. Architecture

### Core (`GridSystem.Core`)
- `GridShapeData`: serialized width, height, `bool[]` cells, pivot. `GridShape` precomputes 4 rotations as pivot-relative offsets; clockwise `(x, y) → (-y, x)`. It also precomputes `GetColumns(rotation)` — one offset per occupied column, all on row zero.
- `GridModel`: flat `int[]` occupancy of owner ids. `Fill` / `Clear` are internal; `CanPlace(span, origin, ignoredA, ignoredB)`.
- `GridItem`: `Id`, `Definition`, `InventoryId`, `Origin`, `Rotation` (setters internal). It has no `Cells`: the footprint depends on the inventory, not on the item.
- `GridInventory`:
  - `IsBar` (`Model.Height == 1`) makes a single row inventory behave as a hotbar: an item occupies one cell per column and its shape rows are ignored, so an item of any height fits into the row. `GridSurface.IsBar` mirrors the rule on the view side, where `UIItemView` centres items by their own bounds. There is no separate flag, the rule is derived from the height;
  - `TryAdd` requires `InventoryId == EMPTY`;
  - `TryMove` / `TryRemove` check ownership; `TryTransfer` is atomic between inventories;
  - `TryFindPlacement` tries all rotations;
  - `Clear`;
  - R3 `Subject`s `Added`, `Removed`, `Moved`.
- `IGridItemDefinition`: `Id`, `IsRotatable`, `Shape`. `SequentialIdProvider` gives item ids.

### Runtime (`GridSystem.Runtime`)
- **Composition** (DI agnostic):
  - `IGridRegistrar` (`Register<T>()`, `RegisterInstance<T>()`) is implemented by each integration.
  - `GridSystemComposition.Install(registrar, settings, poolRoot)` registers all core services.
  - `GridSystemContext` (MonoBehaviour: `_settings`, `_poolRoot`, `_modules`) installs the composition plus `GridModule`s.
  - `GridEntryPoint` calls every `IGridInitializable`; each integration has its own entry point adapter.
- **Settings**: `GridSystemSettings` SO = `ItemDefinitionCatalog` + `MergeRules` + `PointerInputConfig`.
- **Definitions**: `ItemDefinition` SO (`_id` = asset GUID, `_isRotatable`, `_shape`), `ItemDefinitionCatalog` (lookup by id, used by saves), `GridDefinition` SO (width, height).
- **Pointer**: `PointerInputConfig` (Point, Press, Rotate `InputActionReference`). `InputSystemPointerInput` publishes `PointerMoved`, `PointerButton` (started / canceled) and `RotateRequested`.
- **Messaging**: isolated MessagePipe `BuiltinContainerBuilder` (`GridMessageBus`). Struct messages: pointer, `ItemAdded/Removed/Moved`, `InventoryDestroyed`, `ItemDragStarted/Updated/Ended`, `ItemHover`, `ItemsMerged`, `SurfaceOpened/Closed`.
- **Surfaces**:
  - `GridSurface` (`_views` catalog, `_itemsRoot`, `_cellsRoot`, `_cellPrefab`, `_cellSize`, `_spacing`, `_priority`) with `UIGridSurface` (`_rect`, `_eventCamera`; sets its rect size) and `WorldGridSurface` (`_camera`; XZ plane of the object, object is the grid center).
  - Screen-to-cell conversion is analytic: no raycasts, no colliders, no EventSystem.
  - `SurfaceService` opens and closes an inventory on a surface (`ISurfaceService`) and picks the top-priority surface under the pointer (`ISurfaceLocator`). It closes surfaces on `InventoryDestroyed`.
  - `GridSurfacePresenter` (internal) spawns views and owns the drag preview.
- **Views**:
  - `ItemView` (`_motion`, `_highlighter`, `_body`) with `UIItemView` (`_rect`; size and pivot from shape) and `WorldItemView` (`_model` centered on the shape).
  - `ItemViewCatalog` (definition → prefab) and `ItemViewPool`.
  - Motions: Instant, PrimeTween, DOTween. Highlighters: `GraphicItemHighlighter`, `RendererItemHighlighter` (MaterialPropertyBlock, `_colorProperty`: `_Color` Built-in, `_BaseColor` SRP).
- **Interaction**:
  - `ItemDragService` handles drag; the drop chain is `IItemDropHandler` ordered: `MergeDropHandler` 100, then `PlaceDropHandler` 1000. The drop is revalidated at release, and the drag is cancelled on `ItemRemoved` or `SurfaceClosed`.
  - `ItemHoverService` publishes `ItemHover`.
- **Inventories**: `InventoryService` (create / destroy, relays model events to the bus), `GridItemFactory`.
- **Persistence**: `PlayerPrefsSaveStorage`, `InventoryPersistence` (JSON snapshot; `TryLoad` clears first), `InventorySnapshot`.

### Integrations
Each package has three parts:
- a registrar;
- an entry point adapter (Zenject `IInitializable`, VContainer `RegisterEntryPoint`, Reflex eager resolve);
- an `InstallGridSystem(context)` extension.

Ready components:
- `ZenjectGridInstaller` (MonoInstaller, add to `SceneContext → Mono Installers`);
- `ReflexGridInstaller` (`IInstaller` on or under a `ContainerScope`);
- `GridSystemLifetimeScope` (LifetimeScope).

All three have one field, `_context`.

### Samples (`GridSystem.Samples`, demo only)
- `SampleModule : GridModule` (`_config`, `_camera`). It registers the config, the camera, `SampleContent`, `SampleWindowPresenter`, `SampleAutoSave`, `SampleBootstrap`, `ItemTooltipPresenter`.
- `SampleSceneConfig` (asset): `_uiPrefab`, `_worldPrefab`, four `SampleInventory` (key, `GridDefinition`, start items), three `InputActionReference`. Holds no scene reference, so all three container scenes share it.
- `SampleContent` spawns the UI and world prefabs through `IGridObjectFactory` on first use, so nothing depends on initialization order. `SampleSceneRoot` on the UI prefab exposes its surfaces, windows and tooltip.
- `SampleItemDefinition : ItemDefinition, IItemDescription` (display name, damage, attack speed, range).
- `Samples/SETUP.md` is the manual setup guide reproducing the v1 scene.

## 4. Old implementation (`Examples`) — what exists and how it maps

Old scene `TestInventoryScene`:

| Object | Old components | v2 replacement |
|---|---|---|
| `SceneContext` → `GameInstaller` | Zenject `SceneContext`, `GameInstaller` (8 configs) | DI object + `GridSystemContext` + `SampleModule` |
| `InteractionSystem` | 3× `ExampleInteractionSystem` (inventory type, toggle action I/E/C, open on start = false) | sample windows logic (not implemented for 3 windows yet) |
| `WindowsSystem` (prefab) | `Canvas`, `CanvasScaler`, `GridLayoutGroup` (300×300 cells), `GraphicRaycaster`, `GameObjectContext`, `RootWindowInstaller`, `InventoriesView`, `ExampleSelectableSpawnSystem`, `DescriptionView` | `Canvas` + `CanvasScaler` (+ `GridLayoutGroup` can stay: 3×3 windows of 100 px = 300) |
| `MainInventory`, `OthersInventories` | `GridLayoutGroup`, `ContentSizeFitter`, `Image`, `UIGridView`; children `ItemsParent`, `InventoryHeader` | `Image` + `UIGridSurface`, new `CellsParent` above `ItemsParent` |
| `OutsideParent` | `LayoutElement` (ignore), `HorizontalLayoutGroup`; child `HelpWindow` | hand surface (`UIGridSurface`) |
| `TempParent`, `PoolParent` | `LayoutElement` | temp not needed; pool root |
| `InventoryItemView` | `LayoutElement`, `VerticalLayoutGroup`, rows `DataContainer` (`Description` label + `Data` value, Legacy `Text`) | description view (TMP) |
| `Environment/Main Camera` | + `PhysicsRaycaster` | not needed |
| `Environment/WorldPrefab` (prefab) | `GameObjectContext`, `RootWorldInstaller`, `WorldGridView`, `MeshCollider`; `Ground/CellsParent`, `PoolParent` | `WorldGridSurface` |

`GameObjectContext` exists only in Zenject; prefabs used by all DI must not contain it.

Old data (`Examples/ScriptableObjects`), values to keep:

| Asset | Content | v2 type |
|---|---|---|
| `KatanaData`, `Bullet 1`, `PistolData`, `LaserTowerData`, `Wall` (`ItemData`) | shape (`2` = pivot, `1` = filled; rows top-first): Katana `x/P/x`, Bullet `P`, Pistol `Px/x.`, LaserTower `P`, Wall `Px/x.`; stats Name / AttackSpeed / Damage / Range: KATANA 1/2/3, BULLET 1/2/3, PISTOL 1/2/3, LaserTower 10/1/10, AngleWall 0/0/0 | `SampleItemDefinition` |
| `Bullet 2`, `Bullet 3` | duplicates | delete |
| `MergeConfig` | by `ItemType + Level`: Katana → Bullet, Bullet → Pistol, LaserTower → Wall | `MergeRules` (First = Second = ingredient) |
| `InputConfig` | Drag `DragDrop/Moved`, BeginDrag `DragDrop/LMB_Hold` (Hold), Rotation `DragDrop/Rotation` | `PointerInputConfig` |
| `MainInventory` (`InventoryConfig`) | 3×3, used for Main and Chest | `GridDefinition` |
| `WorldGrid` (`InventoryConfig`) | 10×10 | `GridDefinition` |
| `UIItemsData` / `WorldItemsData` | Katana, Bullet, Pistol / LaserTower, Wall | `ItemViewCatalog` UI / World |
| `SelectableItemConfig` | Bullet, Katana, Pistol, Bullet | hand spawn pool |
| `InventoriesConfig`, `WorldOverlapConfig` | enum map / physics | not needed |

Old prefabs:
- `Items/UI/Katana`, `Ammo `, `Pistol`: root has `UIDraggableObject`, `CanvasGroup`, a transparent raycast `Image`, and a nested `*Visual` (root + `Sprite` Image).
- `Items/World/LaserTower`, `Wall`: meshes, `BoxCollider`, `WorldColorizeProvider` on a transparent `ColorizeItem` overlay (alpha 0 normal, 0.8 green / red).
- `UIPrefabs/GridCell` (UI cell, sprite `GridBackground_2`) and `WorldPrefabs/GridItem` (flat cube cell) can be used as cells.

Input asset: `Examples/Scripts/InputSystem/GameInput.inputactions`:
- `DragDrop`: Moved, LMB_Hold, Rotation;
- `Control`: Inventory `I`, Interaction `E`, SLWorld `C`.

## 5. Lessons from the previous attempt (reverted)

1. Building scenes or prefabs by generators or YAML without Unity produced non-working results. Work in the editor via MCP and verify in Play Mode.
2. Saves: `InventoryPersistence.TryLoad` returns `true` when a key exists, even if the saved ids are unknown or no longer fit the grid. The inventory stays empty and start items are skipped. Fix idea: treat a save as invalid when any item fails to restore (clear, delete the key, return false).
3. `ItemViewCatalog`, `ItemDefinitionCatalog` and `MergeRules` build their dictionaries in `OnEnable`. During asset import they can cache `null` references to not-yet-imported assets. `ItemViewPool.Rent` then threw `ArgumentNullException`. Fix idea: build lazily on first use, reset in `OnEnable` / `OnValidate`.
4. An exception in one `IGridInitializable` aborts the rest; their `Dispose` then throws NRE (e.g., the tooltip presenter). Fix the first error first.
5. Pool root under the Canvas works for UI views. World views must not depend on canvas layout.
6. Use the TMP template serialization from Unity if TMP components are needed. Font asset can stay empty (TMP default).

## 6. Open architectural question (ask the user before acting)

The user dislikes MonoBehaviour plumbing in DI. Today:
- `GridSystemContext` and `GridModule` / `SampleModule` are MonoBehaviours;
- the DI installer only points to `GridSystemContext`;
- the old `GameInstaller` bound all configs directly.

Proposed direction (not approved yet):
- `GridModule` becomes a `ScriptableObject`; modules are listed in `GridSystemSettings`.
- `GridSystemContext` is removed. The DI installer gets `Settings` (SO) plus a list of scene views (surfaces, description view) registered as instances.
- Windows link to scene surfaces through a `SurfaceId` SO key instead of scene references in SO data.
- The pool creates its own root.

Present this to the user and get a decision before migrating scenes.

## 7. Next steps

1. Agree with the user on point 6.
2. Fix the issues from section 5 in code, then confirm that it compiles with Unity (console clean).
3. Done. The Zenject sample scene now reproduces the v1 scene: three toggled windows (Main `I`, Chest `E`, World `C`, closed on start), the hand always open as a 5×1 bar, merge rules `Bullet+Bullet → Pistol`, `Katana+Katana → Bullet`, `LaserTower+LaserTower → Wall`.
   The description panel has **2 rows**, not 4: v1 declares four `Text` fields but `DescriptionView` only ever assigns `NameValue` and `DamageValue`, so attack speed and range were never shown.
4. Done. `New/` and the Zenject scene no longer reference `Examples/` at all, so `Examples` can be deleted.
   - Art moved with its `.meta`, so GUIDs and the v1 scene survive: `GameInput.inputactions` into `Samples/Data`,
     textures, materials and the `*Visual` prefabs into `Samples/Art`.
   - The item views were variants of v1 prefabs, which dragged in v1 scripts (`UIDraggableObject`,
     `WorldColorizeProvider`) through the base. They were unpacked in place, keeping their GUIDs.
   - `WindowsSystem` and `WorldPrefab` instances were unpacked in the scene and saved as
     `Samples/Prefabs/SampleWindowsSystem.prefab` and `SampleWorldGrid.prefab`.
     `GridSystemContext` and `SampleModule` moved from the DI object into the UI prefab, so the whole
     sample configuration lives in the prefab instead of in scene overrides.
5. The Zenject scene is done and holds no content at all: `EventSystem`, `SceneContext` and `Environment`.
   The UI and the world grid are spawned at startup through `IGridObjectFactory`, so the container injects
   them exactly like scene objects. A Reflex or VContainer scene is the same three objects with a different
   DI component; only `_context`, `_config` and `_camera` are wired per scene.
   The scene file still lives in `Examples/Scenes`, move it to `New/Samples` when `Examples` goes away.
6. Update `Samples/SETUP.md` and `README.md`. Give the user a Play Mode checklist. Commit only after the user confirms.
