# Сцены примера

Готовые сцены: `Samples/Content/Scenes/GridSystem_Zenject`, `GridSystem_Reflex`, `GridSystem_VContainer`.
Собраны из старой `TestInventoryScene` и её префабов: GUID ассетов сохранены, заменены только компоненты.

Перед запуском:
1. `Project Settings → Player → Active Input Handling` = `Input System Package (New)` или `Both`.
2. TMP Essential Resources импортированы.
3. Мешают старые сохранения → `Edit → Clear All PlayerPrefs`.

Управление (старый `Samples/Input/GameInput`):

| Клавиша | Действие |
|---|---|
| `I` | `MainInventory` (3×3) |
| `E` | `OthersInventories`, сундук (3×3) |
| `C` | `WorldPrefab`, 3D-сетка (10×10) |
| ЛКМ (удерживать) | перенос |
| `R` | поворот при переносе |

`OutsideParent` — рука (6×2), открыта всегда, при первом запуске получает Bullet, Katana, Pistol.

---

## 1. Объекты, общие для всех DI

| Объект | Компоненты | Было |
|---|---|---|
| `InteractionSystem` | `GridSystemContext`, `SampleModule` | 3× `ExampleInteractionSystem` |
| `Environment/Main Camera` | `Camera`, `AudioListener` | + `PhysicsRaycaster` |
| `Environment/Directional Light` | `Light` | — |
| `Environment/WorldPrefab` (префаб) | `WorldGridSurface` | `GameObjectContext`, `RootWorldInstaller`, `WorldGridView`, `MeshCollider` |
| `WorldPrefab/Ground` | `MeshFilter`, `MeshRenderer` | — |
| `WorldPrefab/Ground/CellsParent` | — | — |
| `WorldPrefab/ItemsParent` | — | новый (раньше предметы лежали в `Ground`) |
| `EventSystem` | `EventSystem`, `InputSystemUIInputModule` | — |
| `WindowsSystem` (префаб) | `Canvas`, `CanvasScaler`, `GridLayoutGroup`, `GraphicRaycaster` | + `GameObjectContext`, `RootWindowInstaller`, `InventoriesView`, `ExampleSelectableSpawnSystem`, `DescriptionView` |
| `WindowsSystem/MainInventory` | `Image`, `UIGridSurface` | `GridLayoutGroup`, `ContentSizeFitter`, `Image`, `UIGridView` |
| `MainInventory/InventoryHeader` (префаб) | — | — |
| `MainInventory/CellsParent` | — | новый |
| `MainInventory/ItemsParent` | `LayoutElement` | — |
| `WindowsSystem/OthersInventories` | как `MainInventory` | как `MainInventory` |
| `WindowsSystem/OutsideParent` | `LayoutElement`, `Image`, `UIGridSurface` | `LayoutElement`, `HorizontalLayoutGroup` |
| `OutsideParent/CellsParent`, `ItemsParent` | — | новые |
| `OutsideParent/HelpWindow` (префаб) | — | — |
| `WindowsSystem/TempParent` | удалён | — |
| `WindowsSystem/PoolParent` | `LayoutElement` | — |
| `WindowsSystem/InventoryItemView` | `LayoutElement`, `VerticalLayoutGroup`, `ItemDescriptionView` | `DescriptionView` был на `WindowsSystem` |
| `InventoryItemView/DataContainer … (3)` | `Image`; дети `Description`, `Data` — `TextMeshProUGUI` | 2 строки, Legacy `Text` |

`GridLayoutGroup` на `WindowsSystem` оставлен: он раскладывает окна по 300×300. Окна 3×3 по 100 совпадают с этим размером.

## 2. Объекты DI

| DI | Объект | Компоненты |
|---|---|---|
| Zenject | `SceneContext` | `SceneContext`: `Mono Installers` = [`GameInstaller`] |
| Zenject | `SceneContext/GameInstaller` | `ZenjectGridInstaller`: `Context` = `InteractionSystem` (было `GameInstaller`) |
| Reflex | `SceneScope` | `ContainerScope` |
| Reflex | `SceneScope/GameInstaller` | `ReflexGridInstaller`: `Context` = `InteractionSystem` |
| VContainer | `LifetimeScope` | `GridSystemLifetimeScope`: `Context` = `InteractionSystem`, `Auto Run` ✔ |

`GameObjectContext` удалён из префабов: в Reflex и VContainer его нет, префабы общие для всех DI.

## 3. Поля

### GridSystemContext — `InteractionSystem`
| Поле | Значение |
|---|---|
| `Settings` | `Content/GridSystemSettings` |
| `Pool Root` | `WindowsSystem/PoolParent` |
| `Modules` | `SampleModule` с этого объекта |

### SampleModule — `InteractionSystem`
| Поле | Значение |
|---|---|
| `Description` | `WindowsSystem/InventoryItemView` |
| `Data → Hand` | `Hand` / `HandGrid` / `OutsideParent` |
| `Data → Hand Spawn Pool`, `Count` | Bullet, Katana, Pistol / 3 |

| `Windows` | Inventory (Key / Definition / Surface) | Root | Toggle | Start Items |
|---|---|---|---|---|
| 0 | `Main` / `MainGrid` / `MainInventory` | `MainInventory` | `Control/Inventory` | — |
| 1 | `Chest` / `MainGrid` / `OthersInventories` | `OthersInventories` | `Control/Interaction` | Katana (0,1), Bullet (2,0), Pistol (1,1) |
| 2 | `World` / `WorldGrid` / `WorldPrefab` | `WorldPrefab` | `Control/SLWorld` | LaserTower (2,2), LaserTower (5,5), Wall (7,3) |

### UIGridSurface — `MainInventory`, `OthersInventories`, `OutsideParent`
`Views` `UIItemViews`, `Items Root` / `Cells Root` — свои дети, `Cell Prefab` `GridCell`, `Cell Size` (100, 100), `Spacing` (0, 0), `Priority` 10, `Rect` — сам объект.

### WorldGridSurface — `WorldPrefab`
`Views` `WorldItemViews`, `Items Root` `ItemsParent`, `Cells Root` `Ground/CellsParent`, `Cell Prefab` `GridItem`, `Cell Size` (1, 1), `Spacing` (0, 0), `Priority` 0, `Camera` — `Main Camera` (override в сцене).

### ItemDescriptionView — `InventoryItemView`
`Root` — сам объект, `Name` / `Damage` / `Attack Speed` / `Range` — `Data` строк `DataContainer`, `(1)`, `(2)`, `(3)`.

---

## 4. Ассеты (`Samples/Content`)

| Ассет | Тип | Было |
|---|---|---|
| `Definitions/Katana`, `Bullet`, `Pistol`, `LaserTower`, `Wall` | `SampleItemDefinition` | `KatanaData`, `Bullet 1`, `PistolData`, `LaserTowerData`, `Wall` (`ItemData`) |
| `Definitions/MergeRules` | `MergeRules` | `MergeConfig` |
| `Definitions/ItemDefinitionCatalog` | `ItemDefinitionCatalog` | новый |
| `Input/PointerInputConfig` | `PointerInputConfig` | `InputConfig` |
| `Grids/MainGrid` (3×3), `WorldGrid` (10×10) | `GridDefinition` | `MainInventory`, `WorldGrid` (`InventoryConfig`) |
| `Grids/HandGrid` (6×2) | `GridDefinition` | новый |
| `Catalogs/UIItemViews`, `WorldItemViews` | `ItemViewCatalog` | `UIItemsData`, `WorldItemsData` |
| `GridSystemSettings` | `GridSystemSettings` | новый |
| `Prefabs/Items/UI/Katana`, `Bullet`, `Pistol` | `UIItemView`, `PrimeTweenItemMotion`, `GraphicItemHighlighter` | `Items/UI/*` (`UIDraggableObject`) |
| `Prefabs/Items/World/LaserTower`, `Wall` | `WorldItemView`, `PrimeTweenItemMotion`, `RendererItemHighlighter`; меши в `Model/Visual` | `Items/World/*` (`WorldColorizeProvider`, `BoxCollider`) |
| `Prefabs/Items/Visual/*`, `Prefabs/Cells/GridCell`, `GridItem`, `Textures`, `Materials` | без изменений | `Examples` |

Katana, Bullet и Pistol — только UI. LaserTower и Wall — только 3D, как в старом примере. Предмет переносится только на поверхность, в каталоге которой он есть.

## 5. Проверка в Play Mode
1. Консоль без ошибок, внизу рука с 3 предметами.
2. `I`, `E`, `C` открывают окна, перенос между рукой, Main и сундуком.
3. Katana + Katana → Bullet, Bullet + Bullet → Pistol, LaserTower + LaserTower → Wall (на 3D-сетке).
4. Наведение на предмет — `InventoryItemView` справа.
5. Перезапуск — раскладка восстановлена.
