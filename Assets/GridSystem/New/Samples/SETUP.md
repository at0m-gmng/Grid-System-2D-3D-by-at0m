# Ручная настройка сцены примера

## Автоматически
`Tools → Grid System → Samples`:
- `Create Content` — создаёт или обновляет все ассеты и префабы из шагов 1–3 в `Content/`.
- `Create Scene → Reflex | Zenject | VContainer` — создаёт контент, новую сцену из шагов 4–6 с выбранным DI и сохраняет её в `Content/Scenes/GridSystem_<DI>.unity`. Пункт есть только для установленных DI. Для Reflex при отсутствии создаётся `Assets/Resources/ReflexSettings`.

Перед первым запуском: `Window → TextMeshPro → Import TMP Essential Resources` (шрифт описания предмета). Генератор перезаписывает префабы и ассеты в `Content/`, ручные правки в них не сохраняются.

Ниже — то же самое руками, с описанием каждого объекта и компонента.

## Вручную

Сцена повторяет старый пример (`Examples`): те же имена объектов, тот же `GameInput`, клавиши `I` / `E` / `C`.
Состав: UI-инвентарь `MainInventory` (6×4, `I`), сундук `OthersInventories` (4×4, `E`), «рука» `OutsideParent` (6×2, всегда открыта, внизу экрана), 3D-сетка `WorldPrefab` (10×10, `C`), панель описания `InventoryItemView`. Drag&drop между всеми (включая UI ↔ 3D), поворот, merge, сохранения.

Соответствие старой сцене:

| Старое | Новое |
|---|---|
| `SceneContext` + `GameInstaller` + конфиги | `GridSystem` (`GridSystemContext` + `SampleModule`) + объект DI |
| `InteractionSystem` (3× `ExampleInteractionSystem`) | `SampleModule → Data → Windows` (3 окна с `Toggle`) |
| `WindowsSystem` (`GameObjectContext`, `RootWindowInstaller`, `InventoriesView`, ...) | `WindowsSystem` (только `Canvas` + `CanvasScaler`) |
| `MainInventory` / `OthersInventories` / `OutsideParent` | те же имена, внутри `Grid` с `UIGridSurface` |
| `TempParent` | не нужен: превью drag живёт на поверхности |
| `PoolParent` | `PoolParent` в корне сцены |
| `InventoryItemView` / `DescriptionView` | `InventoryItemView` (`ItemDescriptionView`) |
| `ExampleSelectableSpawnSystem` | `SampleModule → Data → Hand Spawn Pool / Count` |
| `WorldPrefab` (`GameObjectContext`, `RootWorldInstaller`, `WorldGridView`, `MeshCollider`) | `WorldPrefab` (`WorldGridSurface`, коллайдер не нужен) |
Шаги 0–5 общие для всех DI, шаг 6 — отдельно для Reflex, Zenject и VContainer. Каждая DI-сцена — копия общей сцены с другим объектом DI.

Все новые ассеты кладём в `Assets/GridSystem/New/Samples/Content/` (дальше — `Content/`).

---

## 0. Предусловия

1. Установлены: UniTask, MessagePipe, R3 (NuGet `R3` + `R3.Unity`), Input System, PrimeTween, нужный DI.
2. `Project Settings → Player → Active Input Handling` = `Input System Package (New)` или `Both`.
3. Проект на Built-in Render Pipeline (`GraphicsSettings → Custom Render Pipeline` пусто) → у 3D-подсветки свойство цвета `_Color`. Для URP/HDRP — `_BaseColor`.
4. Если сохранения от прошлых запусков мешают: `Edit → Clear All PlayerPrefs`.

---

## 1. Ввод

Используется `Assets/GridSystem/New/Samples/Input/GameInput.inputactions` — ассет старого примера, перенесён с тем же GUID (старая сцена `Examples` продолжает на него ссылаться).

| Map | Action | Тип | Binding | Назначение |
|---|---|---|---|---|
| `DragDrop` | `Moved` | Value / Vector2 | `<Mouse>/position` | позиция указателя |
| `DragDrop` | `LMB_Hold` | Button, Hold | `<Mouse>/leftButton` | захват/отпускание (`started` / `canceled`) |
| `DragDrop` | `Rotation` | Button | `<Keyboard>/r` | поворот при drag |
| `Control` | `Inventory` | Button | `<Keyboard>/i` | окно `MainInventory` |
| `Control` | `Interaction` | Button | `<Keyboard>/e` | окно `OthersInventories` (сундук) |
| `Control` | `SLWorld` | Button | `<Keyboard>/c` | 3D-сетка `WorldPrefab` |

1. `InputActionReference` для каждого действия — вложенные ассеты под `GameInput` (раскрыть стрелкой в Project).
2. `Content/Input` → `Create → Grid System → Pointer Input Config` → `PointerInputConfig`:
   - `Point` = `DragDrop/Moved`, `Press` = `DragDrop/LMB_Hold`, `Rotate` = `DragDrop/Rotation`.

---

## 2. Данные

### 2.1 Предметы
`Content/Definitions` → `Create → Grid System → Samples → Sample Item Definition`, по одному на предмет.
Форма: задать `Width`/`Height`, ЛКМ — клетка вкл/выкл, ПКМ — pivot (оранжевая «P», точка привязки и центр поворота). Строка 0 — верхняя.

| Ассет | Display Name | Is Rotatable | Width × Height | Клетки (строки сверху вниз, `P` — pivot, `x` — занята, `.` — пусто) | Damage / Attack Speed / Range |
|---|---|---|---|---|---|
| `Katana` | Katana | ✔ | 1 × 3 | `x` / `P` / `x` | 30 / 1.2 / 1.5 |
| `Bullet` | Bullet | ✔ | 1 × 1 | `P` | 5 / 0 / 0 |
| `Pistol` | Pistol | ✔ | 2 × 2 | `P x` / `x .` | 15 / 2 / 20 |
| `LaserTower` | Laser Tower | ✔ | 1 × 1 | `P` | 50 / 0.5 / 8 |
| `Wall` | Wall | ✔ | 2 × 2 | `P x` / `x .` | 0 / 0 / 0 |

`Id` заполняется автоматически GUID-ом ассета — не трогать.

### 2.2 Каталог
`Content/Definitions` → `Create → Grid System → Item Definition Catalog` → `ItemDefinitionCatalog`, `Items` = все 5 предметов.

### 2.3 Merge
`Content/Definitions` → `Create → Grid System → Merge Rules` → `MergeRules`:

| First | Second | Result |
|---|---|---|
| Katana | Katana | Bullet |
| Bullet | Bullet | Pistol |
| LaserTower | LaserTower | Wall |

Результат ставится в pivot целевого предмета и должен поместиться в клетки обоих ингредиентов + свободные клетки рядом.

### 2.4 Размеры сеток
`Content/Grids` → `Create → Grid System → Grid Definition`:

| Ассет | Width | Height |
|---|---|---|
| `MainGrid` | 6 | 4 |
| `ChestGrid` | 4 | 4 |
| `HandGrid` | 6 | 2 |
| `WorldGrid` | 10 | 10 |

### 2.5 Настройки
`Content` → `Create → Grid System → Settings` → `GridSystemSettings`:
`Items` = `ItemDefinitionCatalog`, `Merge Rules` = `MergeRules`, `Input` = `PointerInputConfig`.

---

## 3. Префабы

### 3.1 UI-ячейка `Content/Prefabs/UI/UI_Cell`
1. Объект с `RectTransform`: anchors `(0.5, 0.5)`–`(0.5, 0.5)`, pivot `(0.5, 0.5)`.
2. `Image`: Sprite = `Examples/Textures/Grid/GridBackground`, `Raycast Target` выкл.
3. Размер выставит поверхность (Cell Size), позицию и поворот тоже.

### 3.2 3D-ячейка `Content/Prefabs/World/World_Cell`
1. Пустой корень `World_Cell` (поворот корня перезаписывает поверхность).
2. Дочерний `Quad`: Position `(0, 0.001, 0)`, Rotation `(90, 0, 0)`, Scale `(1, 1, 1)`, материал `Examples/Materials/GridCellMaterial`, `MeshCollider` удалить.

### 3.3 UI-вьюха предмета `Content/Prefabs/UI/UI_<Item>` (для каждого из 5)
```
UI_Pistol            RectTransform, UIItemView, PrimeTweenItemMotion, GraphicItemHighlighter
└─ Body              RectTransform, Image
```
1. `UI_Pistol` → `RectTransform`: anchors `(0.5, 0.5)`–`(0.5, 0.5)` (НЕ stretch). Размер и pivot считаются из формы автоматически.
2. `Body` → `RectTransform`: anchors `(0, 0)`–`(1, 1)`, Left/Right/Top/Bottom = 0. `Image`: Sprite из `Examples/Textures/Items` (Katana, Ammo для Bullet, Pistol; для LaserTower/Wall — любой спрайт), `Preserve Aspect` вкл, `Raycast Target` выкл. Спрайт рисуется в неповёрнутой ориентации формы.
3. `PrimeTweenItemMotion`: `Target` = `UI_Pistol`. Без PrimeTween — `InstantItemMotion` (или `DOTweenItemMotion` при `DOTWEEN`).
4. `GraphicItemHighlighter`: `Graphic` = `Body` (Image). Palette по умолчанию: белый / зелёный / красный.
5. `UIItemView`: `Motion` = `PrimeTweenItemMotion`, `Highlighter` = `GraphicItemHighlighter`, `Body` = `Body`, `Rect` = `UI_Pistol`.

### 3.4 3D-вьюха предмета `Content/Prefabs/World/World_<Item>` (для каждого из 5)
```
World_Wall           Transform, WorldItemView, PrimeTweenItemMotion, RendererItemHighlighter
└─ Model             Transform (Body и Model одновременно)
   └─ Cube ...       MeshRenderer
```
1. Корень — пустой. Модель отцентрована по габаритам формы, 1 юнит = 1 клетка, низ на `y = 0`. Строки формы идут по `-Z`, столбцы по `+X`.
2. Кубы (`Scale (0.9, 0.8, 0.9)`, `Y = 0.4`, коллайдеры удалить):
   - Katana (1×3): `(0, 0.4, 1)`, `(0, 0.4, 0)`, `(0, 0.4, -1)`.
   - Bullet, LaserTower (1×1): `(0, 0.4, 0)` (LaserTower выше: Scale Y = 1.6, Y = 0.8).
   - Pistol, Wall (2×2, клетки `P x / x .`): `(-0.5, 0.4, 0.5)`, `(0.5, 0.4, 0.5)`, `(-0.5, 0.4, -0.5)`.
3. Материал кубов — любой Standard (например `Examples/Materials/GridItemMaterial`).
4. `PrimeTweenItemMotion`: `Target` = корень.
5. `RendererItemHighlighter`: `Renderers` = все MeshRenderer кубов, `Color Property` = `_Color` (Built-in).
6. `WorldItemView`: `Motion`, `Highlighter`, `Body` = `Model`, `Model` = `Model`.

### 3.5 Каталоги вьюх
`Content/Catalogs` → `Create → Grid System → Item View Catalog`:
- `UIItemViews`: 5 записей `Definition → UI_<Item>`.
- `WorldItemViews`: 5 записей `Definition → World_<Item>`.

Предмет перетаскивается только на поверхности, в каталоге которой он есть. Чтобы работал перенос UI ↔ 3D, предмет должен быть в обоих каталогах. Результаты merge тоже обязаны быть в каталоге.

---

## 4. Сцена (общая часть)

`Content/Scenes` → новая пустая сцена `GridSystem_Common` (потом копируется под каждый DI). Объекты по умолчанию (`Main Camera`, `Directional Light`) удалить — они создаются ниже внутри `Environment`.

Итоговая иерархия:
```
Environment
├─ Main Camera
├─ Directional Light
└─ WorldPrefab               WorldGridSurface
   ├─ Cells
   └─ Items
WindowsSystem                Canvas, CanvasScaler
├─ MainInventory             Image (фон окна)
│  └─ Grid                   UIGridSurface
│     ├─ Cells
│     └─ Items
├─ OthersInventories         Image (фон окна)
│  └─ Grid                   UIGridSurface
│     ├─ Cells
│     └─ Items
├─ OutsideParent             Image (фон окна)
│  └─ Grid                   UIGridSurface
│     ├─ Cells
│     └─ Items
└─ InventoryItemView         ItemDescriptionView
   └─ Popup                  Image, VerticalLayoutGroup
      ├─ DataContainer       ├─ Label "Name:"          └─ Value
      ├─ DataContainer       ├─ Label "Damage:"        └─ Value
      ├─ DataContainer       ├─ Label "Attack Speed:"  └─ Value
      └─ DataContainer       ├─ Label "Range:"         └─ Value
PoolParent                   (выключен)
GridSystem                   GridSystemContext, SampleModule
```
Все UI-объекты — слой `UI`.

### 4.1 Environment
1. Пустой `Environment` в корне, Position `(0, 0, 0)`.
2. `Main Camera` (дочерний): Tag `MainCamera`, `Camera`, `AudioListener`, Position `(0, 12, -9)`, Rotation `(55, 0, 0)`.
3. `Directional Light` (дочерний): `Light` Type `Directional`, Rotation `(50, -30, 0)`.

### 4.2 WorldPrefab (3D-сетка)
1. Пустой `WorldPrefab` в `Environment`, Position `(0, 0, 0)`. Дочерние пустые `Cells`, `Items`.
2. `WorldGridSurface`: `Views` = `WorldItemViews`, `Items Root` = `Items`, `Cell Prefab` = `World_Cell`, `Cells Root` = `Cells`, `Cell Size` = `(1, 1)`, `Spacing` = `(0.05, 0.05)`, `Priority` = `0`, `Camera` = `Main Camera`.
3. Сетка лежит в локальной плоскости XZ объекта, объект — её центр. `MeshCollider` не нужен: пикинг — пересечение луча с плоскостью.
4. Объект одновременно корень окна `World`: выключается/включается по `C`.

### 4.3 WindowsSystem (Canvas)
1. `GameObject → UI → Canvas`, переименовать в `WindowsSystem`. `EventSystem`, созданный вместе с ним, можно удалить — плагину он не нужен (пикинг математический), только для кнопок.
2. `Canvas`: `Render Mode` = `Screen Space - Overlay`.
3. `CanvasScaler`: `UI Scale Mode` = `Scale With Screen Size`, `Reference Resolution` = `1920 × 1080`, `Match` = `0.5`.
4. `GraphicRaycaster` можно удалить.

### 4.4 Окна MainInventory / OthersInventories / OutsideParent
Размер окна = размер сетки + 40 по каждой оси. Размер сетки = `клеток × 84 − 4`.

| Окно | Anchors (min = max) | Pos X, Y | Width × Height |
|---|---|---|---|
| `MainInventory` | `(0.5, 0.5)` | `-330, 40` | `540 × 372` |
| `OthersInventories` | `(0.5, 0.5)` | `330, 40` | `372 × 372` |
| `OutsideParent` | `(0.5, 0)` | `0, 150` | `540 × 204` |

Для каждого окна:
1. `GameObject → UI → Image` в `WindowsSystem`, имя из таблицы, `RectTransform` из таблицы, pivot `(0.5, 0.5)`.
2. `Image`: Color `(0.1, 0.1, 0.12, 0.85)`, `Raycast Target` выкл.
3. Дочерний пустой UI-объект `Grid`: anchors `(0.5, 0.5)`–`(0.5, 0.5)`, pivot `(0.5, 0.5)`, Position `(0, 0)`. Размер выставит `UIGridSurface` сам.
4. В `Grid` дочерние `Cells` и `Items`: anchors `(0, 0)`–`(1, 1)`, Left/Right/Top/Bottom = 0, Scale 1. `Cells` выше `Items` — предметы рисуются поверх ячеек.
5. `Grid` → `UIGridSurface`: `Views` = `UIItemViews`, `Items Root` = `Grid/Items`, `Cell Prefab` = `UI_Cell`, `Cells Root` = `Grid/Cells`, `Cell Size` = `(80, 80)`, `Spacing` = `(4, 4)`, `Priority` = `10`, `Rect` = `Grid`, `Event Camera` = пусто (Overlay).

`Priority` 10 у UI выше 0 у мира: клик по окну не уходит в 3D-сетку под ним.
`MainInventory` и `OthersInventories` включаются/выключаются сами по открытию поверхности, начальное active в сцене неважно. `OutsideParent` открыт всегда.

### 4.5 InventoryItemView (описание предмета)
1. Пустой UI-объект `InventoryItemView` в `WindowsSystem` (последним ребёнком — рисуется поверх окон): anchors `(0, 0)`–`(1, 1)`, offsets 0. Без `Image` — не перекрывает ввод.
2. Дочерний `Popup`: anchors `(0.5, 1)`–`(0.5, 1)`, pivot `(0.5, 1)`, Position `(0, -20)`, Size `500 × 250`.
   - `Image`: Color `(0.464, 0.464, 0.464, 1)`, `Raycast Target` выкл.
   - `VerticalLayoutGroup`: Padding 20 со всех сторон, `Control Child Size` и `Child Force Expand` — Width и Height вкл.
3. В `Popup` 4 пустых UI-объекта `DataContainer`. В каждом:
   - `Label`: anchors `(0, 0)`–`(0.5, 1)`, offsets 0, `TextMeshPro - Text (UI)`: текст `Name:` / `Damage:` / `Attack Speed:` / `Range:`, Font Size 24, Color `(0.196, 0.196, 0.196, 1)`, `Raycast Target` выкл.
   - `Value`: anchors `(0.5, 0)`–`(1, 1)`, offsets 0, `TextMeshPro - Text (UI)` с теми же настройками, текст пустой.
4. `InventoryItemView` → `ItemDescriptionView`: `Root` = `Popup`, `Name` / `Damage` / `Attack Speed` / `Range` = `Value` соответствующей строки.

`Popup` выключается на старте и показывается при наведении на предмет.

### 4.6 PoolParent
Пустой объект `PoolParent` в корне сцены, выключен. Сюда возвращаются вьюхи из пула.

### 4.7 GridSystem
Пустой объект `GridSystem` в корне, компоненты `GridSystemContext` и `SampleModule`.

1. `GridSystemContext`: `Settings` = `GridSystemSettings`, `Pool Root` = `PoolParent`, `Modules` = [`SampleModule` с этого же объекта].
2. `SampleModule` → `Description` = `InventoryItemView`.
3. `SampleModule` → `Data`:
   - `Hand`: Key `Hand`, Definition `HandGrid`, Surface = `OutsideParent/Grid`.
   - `Hand Spawn Pool` = Katana, Bullet, Pistol; `Hand Spawn Count` = 3 (без повторов, только при первом запуске).
   - `Windows` — 3 элемента:

| # | Inventory: Key / Definition / Surface | Root | Toggle | Is Opened On Start | Start Items (Definition, Origin, Rotation) |
|---|---|---|---|---|---|
| 0 | `Main` / `MainGrid` / `MainInventory/Grid` | `MainInventory` | `Control/Inventory` | выкл | — |
| 1 | `Chest` / `ChestGrid` / `OthersInventories/Grid` | `OthersInventories` | `Control/Interaction` | выкл | Katana `(0, 1)` 0; Katana `(1, 1)` 0; Bullet `(3, 0)` 0 |
| 2 | `World` / `WorldGrid` / `WorldPrefab` | `WorldPrefab` | `Control/SLWorld` | выкл | LaserTower `(2, 2)` 0; Wall `(5, 5)` 0; LaserTower `(7, 3)` 0 |

`Start Items` ставятся только когда нет сохранения. Сохранение окна — при его закрытии и при выходе из Play Mode.

---

## 5. Сохранить общую сцену
Дальше сцену копируем (`Ctrl+D` в Project) в три: `GridSystem_Reflex`, `GridSystem_Zenject`, `GridSystem_VContainer`. В каждой — ровно один DI из шага 6.

---

## 6. DI

### 6.1 Reflex — `GridSystem_Reflex`
1. Один раз на проект: создать `Assets/Resources`, в нём `Create → Reflex → Settings` (`ReflexSettings` строго в корне `Resources`). `Root Scopes` можно оставить пустым.
2. В сцене: `GameObject → Reflex → SceneScope` — корневой объект с `ContainerScope`.
3. На `SceneScope` (или его дочерний объект) добавить `ReflexGridInstaller`, `Context` = объект `GridSystem`.

### 6.2 Zenject — `GridSystem_Zenject`
1. `GameObject → Zenject → Scene Context`.
2. На `SceneContext` добавить `ZenjectGridInstaller`, `Context` = `GridSystem`.
3. `SceneContext → Mono Installers` → `+` → перетащить `ZenjectGridInstaller`.
4. `ProjectContext` не нужен.

### 6.3 VContainer — `GridSystem_VContainer`
1. Пустой объект `LifetimeScope` в корне, добавить `GridSystemLifetimeScope`.
2. `Context` = `GridSystem`, `Parent` пусто, `Auto Run` вкл.

Свой scope вместо готовых компонентов:
- Reflex: `containerBuilder.InstallGridSystem(context)` в своём `IInstaller`.
- Zenject: `Container.InstallGridSystem(context)` в своём `MonoInstaller`.
- VContainer: `builder.InstallGridSystem(context)` в `Configure` своего `LifetimeScope`.

---

## 7. Проверка в Play Mode
1. Внизу открыта рука `OutsideParent` с 3 разными предметами. Остальные окна закрыты.
2. Перетаскивание внутри руки: превью зелёное — можно, красное — нельзя. `R` во время drag — поворот.
3. `I` — открыть `MainInventory`, перенести предмет из руки и обратно.
4. `E` — открыть сундук: две Katana и Bullet. Перенести в руку или Main.
5. `C` — показать 3D-сетку с башнями и стеной. Перенести предмет из UI на сетку и обратно: над сеткой превью становится 3D-моделью.
6. Katana на Katana → Bullet; Bullet на Bullet → Pistol; LaserTower на LaserTower → Wall.
7. Навести на предмет — вверху экрана `Popup` с параметрами.
8. Закрыть окно или выйти из Play Mode, снова запустить — раскладка восстановлена.

## 8. Частые ошибки
| Симптом | Причина |
|---|---|
| `KeyNotFoundException` в `ItemViewCatalog.GetPrefab` | Предмет (или результат merge) не добавлен в каталог вьюх поверхности, на которой он появляется |
| Ничего не реагирует на мышь | В `PointerInputConfig` не назначены действия, `Point` не `Value/Vector2`, `Active Input Handling` без Input System |
| `I` / `E` / `C` не работают | В `Windows[i].Toggle` не назначено действие `Control/...` |
| Окно не появляется | `Root` окна не тот объект или `Surface` указывает на `Grid` другого окна |
| Описание пустое или не исчезает | В `ItemDescriptionView` поля указывают на `Label`, а не на `Value`; `Root` не `Popup` |
| UI-предмет смещён относительно клеток | У корня вьюхи anchors stretch, или `Body` не растянут на весь корень |
| 3D-модель смещена | Модель не отцентрована по габаритам формы или масштаб не 1 юнит на клетку |
| 3D-ячейки стоят вертикально | Поворот `(90, 0, 0)` задан корню `World_Cell`, а не дочернему Quad |
| 3D-предмет не подсвечивается | Неверный `Color Property` (`_Color` для Built-in, `_BaseColor` для URP/HDRP) |
| UI-предметы под ячейками | `Items` выше `Cells` в иерархии |
| Предмет не переносится на 3D-сетку | Его нет в `WorldItemViews` |
| `NullReferenceException` при старте | Не заполнено поле в `GridSystemContext`, `SampleModule` или поверхности |
