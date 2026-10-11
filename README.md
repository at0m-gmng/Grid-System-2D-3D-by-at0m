<div align="center">

# "Grid System (2D/3D) by at0m"
#### In process

</div>

### Description
- A Unity plugin for inventories and any other systems that need a grid and irregularly shaped items.
- Version 2 lives in `Assets/GridSystem/New`. The old implementation in `Assets/GridSystem/Examples` is kept as a reference example.
- Technology stack: R3, MessagePipe, PrimeTween or DOTween (optional), Unity Input System. Works with Zenject, VContainer or Reflex.

### Unity versions
- Unity 6 or higher.

#### Advantages
- 2D (Canvas) and 3D surfaces, items move freely between them (UI <-> World).
- Complex shapes with a pivot cell and 4 precomputed rotations.
- DI agnostic: one composition description, thin adapters for Zenject, VContainer and Reflex.
- No allocations in hot paths: flat occupancy matrix, O(1) cell lookup, analytic screen-to-cell conversion (no raycasts), pooled views, struct messages.
- Extensible: drop handlers chain, custom surfaces, views, motions, highlighters, storages, modules.

#### Road Map
- Grid expansion at runtime.
- Addressables item view provider.

## Architecture (v2)

```
Input (IPointerInput) ──► ItemDragService ──► IItemDropHandler chain (Merge → Place)
                               │                        │
                       ISurfaceLocator                  ▼
                               │                 GridInventory (Core, pure C#)
                               ▼                        │ R3 Added / Removed / Moved
                     GridSurfacePresenter ◄─────────────┘
                               │ pooled ItemView (Motion + Highlighter)
                               ▼
                     GridSurface (UIGridSurface / WorldGridSurface)

InventoryService ──MessagePipe──► Item*Message, ItemsMerged, ItemHover, Surface*Message ──► any listener
```

| Assembly | Folder | Purpose |
|---|---|---|
| `GridSystem.Core` | `New/Core` | Shapes, occupancy model, items, inventories. No Unity view code. |
| `GridSystem.Runtime` | `New/Runtime` | Definitions, surfaces, views, pointer, drag & drop, merge, persistence, messaging, composition. |
| `GridSystem.Integrations.Zenject` | `New/Integrations/Zenject` | Zenject adapter. |
| `GridSystem.Integrations.VContainer` | `New/Integrations/VContainer` | VContainer adapter. |
| `GridSystem.Integrations.Reflex` | `New/Integrations/Reflex` | Reflex adapter. |
| `GridSystem.Editor` | `New/Editor` | Shape editor in the inspector. |
| `GridSystem.Samples` | `New/Samples` | Sample bootstrap, tooltip, extended item definition. |

Namespaces follow folders (`GridSystem.Runtime.Surfaces`, ...).

### Scripting defines

| Define | Set by | Enables |
|---|---|---|
| `GRID_SYSTEM_PRIMETWEEN` | automatically (asmdef version define of `com.kyrylokuzyk.primetween`) | `PrimeTweenItemMotion` (allocation free, recommended) |
| `DOTWEEN` | DOTween setup, Player Settings → Scripting Define Symbols | `DOTweenItemMotion` (enable DOTween recycling to avoid allocations) |
| `GRID_SYSTEM_ZENJECT` | automatically for `com.svermeulen.extenject`, add manually if Zenject is in Assets | Zenject adapter |
| `GRID_SYSTEM_VCONTAINER` | automatically for `jp.hadashikick.vcontainer` | VContainer adapter |
| `GRID_SYSTEM_REFLEX` | automatically for `com.gustavopsantos.reflex` | Reflex adapter |

Without any tween use `InstantItemMotion`.

### Setup

1. Create assets (`Create/Grid System/...`): `Item Definition` for every item, `Item Definition Catalog`, `Merge Rules`, `Pointer Input Config` (Point: Vector2 pointer position, Press: button, Rotate: button), `Settings` referencing all of them, `Grid Definition` for every grid size, `Item View Catalog` per surface kind (UI, World).
2. Item shape: set Width/Height, LMB toggles a cell, RMB sets the pivot cell. The pivot is the placement anchor and rotation center.
3. Item view prefab:
   - UI: root `RectTransform` with `UIItemView`, a body child with the image, `GraphicItemHighlighter` and a motion component. Size and pivot are calculated from the shape automatically.
   - World: root with `WorldItemView`, model child centered on its bounds (1 unit per cell), `RendererItemHighlighter` and a motion component.
4. Surfaces: `UIGridSurface` (grid `RectTransform`, event camera or none for Overlay) or `WorldGridSurface` (transform is the grid center on local XZ, camera). Assign items root, cells root, cell prefab, cell size, spacing and `Item View Catalog`. Overlapping surfaces are picked by `Priority` (put UI above World).
5. Scene composition: add `GridSystemContext` (settings, inactive pool root, modules) and one adapter:
   - Zenject: `ZenjectGridInstaller` in the SceneContext installers, or `Container.InstallGridSystem(context)`.
   - VContainer: `GridSystemLifetimeScope`, or `builder.InstallGridSystem(context)` in your scope.
   - Reflex: `ReflexGridInstaller` on the scene `ContainerScope` object (or its child), or `builder.InstallGridSystem(context)`.
6. Game code: inject `IInventoryService` (create inventories), `ISurfaceService` (open/close on surfaces), `IInventoryPersistence` (save/load), `IGridItemFactory`, subscribe to messages via `ISubscriber<T>`.

### Extension points
- `GridModule` – scene module registering your services through `IGridRegistrar` (works for every container).
- `IItemDropHandler` – custom drop logic (stacking, swapping, equipment slots); lower `Order` runs first.
- `ItemDefinition` – inherit to add game data (see `SampleItemDefinition`).
- `GridSurface`, `ItemView`, `ItemMotion`, `ItemHighlighter`, `ISaveStorage` – replaceable building blocks.

<div align="center">

### Download
[![Github All Releases](https://img.shields.io/github/downloads/at0m-gmng/Grid-System-2D-3D-by-at0m/total.svg)](https://github.com/at0m-gmng/Grid-System-2D-3D-by-at0m/releases)

</div>
<div align="center">

### Demo

<div align="center" style="font-size: 40px">
<div style="transform: scale(0.35); transform-origin: top center;">

[![GIF](Demo.gif)](Demo.gif)

</div></div></div>
<div style="text-align: left; margin-top: -120%;">

<div align="center">

# ❓ FAQ

</div>

<div>
<details>
<summary style="font-size: 1.6em; font-weight: bold;"><b>How do I create a new entity?</b></summary>
<p> Create/Inventory/Select the desired data</p>
</details>

<details>
<summary style="font-size: 1.6em; font-weight: bold;"><b>How do I set up ItemData?</b></summary>
<p>
1) Create data in the "Create/Inventory/ItemData" path.<br>
2) In the created ItemData, configure Rows and Columns as if your item were rectangular.<br>
3) CellSize is designed for convenience and allows you to customize the visual size.<br>
The location of an element in the table is configured by configuring the position matrix in EditorGrid.<br>
Since our matrix is rectangular, complex objects will have empty cells in addition to the center and occupied cells. This is normal and intended.<br>
<div align="center"><img src="faq3.1.jpg" width="300" title="Подсказка"></div>
4) Next, configure the pivot and dimensions of the object. Also, arrange the element as it is in the EditorGrid.<br>
<div align="center"><img src="faq4.1.jpg" width="300" title="Подсказка"></div><br>
5) For 3D objects, you will need to create a child Transform to implement pivot.<br>
<div align="center"><img src="faq5.jpg" width="300" title="Подсказка"></div><br>
6) Next, you need to add your ItemData to InventoryConfig. For 2D and 3D items, they are different, so be careful.
</p>
</details>

<details>
<summary style="font-size: 1.6em; font-weight: bold;"><b>How do I create a new Grid?</b></summary>
<p> 
1) Create an InventoryConfig and configure its dimensions and cell prefab.<br>
2) Now you need to add your new Grid to InventoriesConfig and assign it the desired type.
</p>
</details>

</div></div>