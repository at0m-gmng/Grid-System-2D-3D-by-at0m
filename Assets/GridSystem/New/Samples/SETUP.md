# Настройка сцены примера

Исходник — копия `Examples/Scenes/TestInventoryScene`. Объекты те же, меняются только компоненты.

Перед началом:
1. `Project Settings → Player → Active Input Handling` = `Input System Package (New)` или `Both`.
2. `Window → TextMeshPro → Import TMP Essential Resources`.
3. Инстансы `WindowsSystem` и `WorldPrefab` → `Prefab → Unpack Completely`. Иначе после удаления `Examples` они сломаются.

Ассеты, которые назначаются в поля:

| Ассет | Тип |
|---|---|
| `GridSystemSettings` | `GridSystemSettings` (Items, Merge Rules, Input) |
| `MainGrid` 6×4, `ChestGrid` 4×4, `HandGrid` 6×2, `WorldGrid` 10×10 | `GridDefinition` |
| `Katana`, `Bullet`, `Pistol`, `LaserTower`, `Wall` | `SampleItemDefinition` |
| `UIItemViews`, `WorldItemViews` | `ItemViewCatalog` |
| `UI_Cell`, `World_Cell` | префаб ячейки |
| `GameInput` (`Samples/Input/GameInput.inputactions`) | Input Actions |

---

## 1. Общие объекты (одинаково для всех DI)

| Объект | Удалить | Добавить |
|---|---|---|
| `InteractionSystem` | 3× `ExampleInteractionSystem` | `GridSystemContext`, `SampleModule` (3.1, 3.2) |
| `Environment/Main Camera` | `PhysicsRaycaster` | — |
| `Environment/Directional Light` | — | — |
| `Environment/WorldPrefab` | `GameObjectContext`, `RootWorldInstaller`, `WorldGridView`, `MeshCollider` | `WorldGridSurface` (3.4), дочерний `ItemsParent` |
| `WorldPrefab/Ground` | `MeshCollider` (если есть) | — |
| `WorldPrefab/PoolParent` | объект целиком | — |
| `EventSystem` | — | — |
| `WindowsSystem` | `GridLayoutGroup`, `GameObjectContext`, `RootWindowInstaller`, `InventoriesView`, `ExampleSelectableSpawnSystem`, `DescriptionView` | — |
| `WindowsSystem/MainInventory` | `UIGridView`, `GridLayoutGroup`, `ContentSizeFitter` | `UIGridSurface` (3.3), дочерний `CellsParent` над `ItemsParent` |
| `WindowsSystem/OthersInventories` | `UIGridView`, `GridLayoutGroup`, `ContentSizeFitter` | `UIGridSurface` (3.3), дочерний `CellsParent` над `ItemsParent` |
| `…/InventoryHeader` (оба окна) | `LayoutElement` | — |
| `WindowsSystem/OutsideParent` | `HorizontalLayoutGroup`, `LayoutElement` | `Image`, `UIGridSurface` (3.3), дочерние `CellsParent`, `ItemsParent` |
| `OutsideParent/HelpWindow` | `LayoutElement` | — |
| `WindowsSystem/TempParent` | объект целиком | — |
| `WindowsSystem/PoolParent` | `LayoutElement` | — (это `Pool Root`, 3.1) |
| `WindowsSystem/InventoryItemView` | `LayoutElement` | `ItemDescriptionView` (3.5). Сделать active, переместить последним ребёнком `WindowsSystem` |
| `InventoryItemView/BG` | — | переместить первым ребёнком |
| `InventoryItemView/DataContainer…/Description`, `…/Data` | `Text` | `TextMeshPro - Text (UI)` (3.5) |
| `InventoryItemView/DataContainer` | — | дублировать до 4 строк (3.5) |

Новые дочерние объекты (`Create Empty`):
- `CellsParent` и `ItemsParent` в окнах — stretch (Anchors `(0, 0)–(1, 1)`, отступы 0).
- `ItemsParent` в `WorldPrefab` — Transform по нулям.

---

## 2. Объекты DI

### Zenject
| Объект | Удалить | Добавить |
|---|---|---|
| `SceneContext` | — | `Mono Installers` = [`GameInstaller`], остальные списки пусто |
| `SceneContext/GameInstaller` | `GameInstaller` | `ZenjectGridInstaller`: `Context` = `InteractionSystem` |

### Reflex
| Объект | Удалить | Добавить |
|---|---|---|
| `SceneContext` (можно переименовать в `SceneScope`) | `SceneContext` (Zenject) | `ContainerScope` |
| `SceneContext/GameInstaller` | `GameInstaller` | `ReflexGridInstaller`: `Context` = `InteractionSystem` |

`ContainerScope` берёт инсталлеры с дочерних объектов. `Assets/Resources/ReflexSettings` уже есть.

### VContainer
| Объект | Удалить | Добавить |
|---|---|---|
| `SceneContext` (можно переименовать в `LifetimeScope`) | `SceneContext` (Zenject) | `GridSystemLifetimeScope`: `Context` = `InteractionSystem`, `Parent` пусто, `Auto Run` ✔ |
| `SceneContext/GameInstaller` | объект целиком | — |

---

## 3. Поля новых компонентов

### 3.1 GridSystemContext — `InteractionSystem`
| Поле | Значение | Было |
|---|---|---|
| `Settings` | `GridSystemSettings` | `GameInstaller`: Input / Merge Config, Ui / World Items Data |
| `Pool Root` | `WindowsSystem/PoolParent` | `RootWindowInstaller`: Pool Paretn |
| `Modules` | 1 элемент: `SampleModule` с этого же объекта | — |

### 3.2 SampleModule — `InteractionSystem`
| Поле | Значение | Было |
|---|---|---|
| `Description` | `InventoryItemView` | `DescriptionView` на `WindowsSystem` |
| `Data → Hand → Key` | `Hand` | — |
| `Data → Hand → Definition` | `HandGrid` | `InventoriesConfig` |
| `Data → Hand → Surface` | `OutsideParent` | `ExampleSelectableSpawnSystem`: Selectable Spawn Transform |
| `Data → Hand Spawn Pool` | `Katana`, `Bullet`, `Pistol` | `SelectableItemConfig` |
| `Data → Hand Spawn Count` | `3` | `SelectableItemConfig` |
| `Data → Windows` | 3 элемента, таблица ниже | 3× `ExampleInteractionSystem` |

| # | Inventory → Key / Definition / Surface | Root | Toggle | Is Opened On Start | Start Items (Definition, Origin, Rotation) |
|---|---|---|---|---|---|
| 0 | `Main` / `MainGrid` / `MainInventory` | `MainInventory` | `Control/Inventory` | ✘ | — |
| 1 | `Chest` / `ChestGrid` / `OthersInventories` | `OthersInventories` | `Control/Interaction` | ✘ | `Katana (0,1) 0`, `Katana (1,1) 0`, `Bullet (3,0) 0` |
| 2 | `World` / `WorldGrid` / `WorldPrefab` | `WorldPrefab` | `Control/SLWorld` | ✘ | `LaserTower (2,2) 0`, `Wall (5,5) 0`, `LaserTower (7,3) 0` |

Было в `ExampleInteractionSystem`:
- `Inventory Type` → `Inventory`;
- `Action Reference` → `Toggle`;
- `Is Open After Creating` → `Is Opened On Start`.

`Root` раньше задавал `InventoriesView → Target Inventories`.

### 3.3 UIGridSurface — `MainInventory`, `OthersInventories`, `OutsideParent`
| Поле | Значение | Было (`UIGridView`) |
|---|---|---|
| `Views` | `UIItemViews` | `GameInstaller`: Ui Items Data |
| `Items Root` | `<окно>/ItemsParent` | Item Parent |
| `Cells Root` | `<окно>/CellsParent` | — (ячейки лежали в самом окне) |
| `Cell Prefab` | `UI_Cell` | Cell Prefab |
| `Cell Size` | `(80, 80)` | `GridLayoutGroup` Cell Size |
| `Spacing` | `(4, 4)` | `GridLayoutGroup` Spacing |
| `Priority` | `10` | — |
| `Rect` | само окно | — |
| `Event Camera` | пусто | — |

Размер окна ставит поверхность. Позиции окон после удаления `GridLayoutGroup` у `WindowsSystem`:

| Окно | Anchors | Pivot | Pos X, Y |
|---|---|---|---|
| `MainInventory` | `(0, 1)–(0, 1)` | `(0, 1)` | `60, -100` |
| `OthersInventories` | `(0, 1)–(0, 1)` | `(0, 1)` | `60, -540` |
| `OutsideParent` | `(0.5, 0)–(0.5, 0)` | `(0.5, 0)` | `0, 60` |

У `Image` окон — `Raycast Target` выкл.

### 3.4 WorldGridSurface — `WorldPrefab`
| Поле | Значение | Было (`WorldGridView`) |
|---|---|---|
| `Views` | `WorldItemViews` | `GameInstaller`: World Items Data |
| `Items Root` | `WorldPrefab/ItemsParent` | Item Parent |
| `Cells Root` | `WorldPrefab/Ground/CellsParent` | Cells Parent |
| `Cell Prefab` | `World_Cell` | Cell Prefab |
| `Cell Size` | `(1, 1)` | — |
| `Spacing` | `(0, 0)` | Spacing |
| `Priority` | `0` | — |
| `Camera` | `Environment/Main Camera` | — (был `PhysicsRaycaster` + `MeshCollider`) |

### 3.5 ItemDescriptionView — `InventoryItemView`
| Поле | Значение | Было (`DescriptionView`) |
|---|---|---|
| `Root` | `InventoryItemView` | Item Pop Up |
| `Name` | `DataContainer/Data` | Name Value |
| `Damage` | `DataContainer (1)/Data` | Damage Value |
| `Attack Speed` | `DataContainer (2)/Data` | Atack Speed Value |
| `Range` | `DataContainer (3)/Data` | Range Value |

Строки `DataContainer`: `Description` = `Name:` / `Damage:` / `Attack Speed:` / `Range:`, `Data` = значение.

TMP-тексты: размер шрифта 24, Color `(0.196, 0.196, 0.196)`, Alignment Left / Middle, `Raycast Target` выкл.

`VerticalLayoutGroup` на `InventoryItemView`:
- Padding 20, Spacing 10;
- Control Child Size: Width ✔, Height ✔;
- Child Force Expand: Width ✔, Height ✔.

---

## 4. Проверка в Play Mode
1. Консоль без ошибок. Внизу рука с 3 предметами, окна закрыты.
2. Перенос внутри руки: превью зелёное / красное, `R` — поворот.
3. `I` — Main, `E` — сундук (Katana ×2, Bullet), `C` — 3D-сетка. Перенос между всеми.
4. Merge: Katana + Katana → Bullet, Bullet + Bullet → Pistol, LaserTower + LaserTower → Wall.
5. Наведение на предмет — `InventoryItemView` с параметрами.
6. Перезапуск — раскладка восстановлена.

## 5. Частые ошибки
| Симптом | Причина |
|---|---|
| `NullReferenceException` при старте | пустое поле в 3.1–3.5 или пустой элемент в `Modules` |
| `I` / `E` / `C` не работают | не назначен `Windows[i] → Toggle` |
| Окно не того размера, ячейки разъехались | остался `GridLayoutGroup` / `ContentSizeFitter` / `HorizontalLayoutGroup` |
| Предметы под ячейками | `ItemsParent` выше `CellsParent` |
| Описание под окнами | `InventoryItemView` не последний в `WindowsSystem` |
| Описание пустое | поля `ItemDescriptionView` указывают на `Description`, а не на `Data` |
| Клик по окну уходит в 3D-сетку | `Priority` окна не больше, чем у `WorldPrefab` |
| Zenject: инсталлер не вызывается | `GameInstaller` не добавлен в `SceneContext → Mono Installers` |
| Reflex: инсталлер не вызывается | `ReflexGridInstaller` не на `ContainerScope` и не на его ребёнке, или нет `Resources/ReflexSettings` |
