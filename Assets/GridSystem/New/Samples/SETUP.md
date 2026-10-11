# Ручная настройка примера Grid System

Пример повторяет старую сцену `Examples/Scenes/TestInventoryScene`: те же объекты (`Environment`, `WorldPrefab`, `WindowsSystem`, `MainInventory`, `OthersInventories`, `OutsideParent`, `InventoryItemView`, `PoolParent`, `SceneContext/GameInstaller`), те же клавиши.

| Клавиша | Действие |
|---|---|
| `I` | открыть/закрыть `MainInventory` (6×4) |
| `E` | открыть/закрыть сундук `OthersInventories` (4×4) |
| `C` | показать/скрыть 3D-сетку `WorldPrefab` (10×10) |
| ЛКМ (удерживать) | взять и перенести предмет |
| `R` во время переноса | поворот на 90° |

`OutsideParent` — «рука» 6×2 внизу экрана, открыта всегда. При первом запуске в неё кладутся 3 разных предмета.
Предмет на такой же предмет → merge. При наведении на предмет показывается `InventoryItemView` с параметрами. Раскладка сохраняется в PlayerPrefs.

Шаги 0–4 общие. Шаг 5 — сцена, общая для всех DI. Шаг 6 — объект DI, единственное отличие сцен Reflex / Zenject / VContainer. Шаг 7 — если начинаешь с копии старой сцены.

Обозначения:
- `Content/` = `Assets/GridSystem/New/Samples/Content/`.
- Поля называются так, как их показывает Inspector.
- RectTransform: `Anchors Min–Max`, `Pivot`, `Pos X, Pos Y`, `Width × Height`. «stretch» = Anchors `(0, 0)–(1, 1)`, Left / Top / Right / Bottom = 0.

---

## 0. Предусловия

1. Установлены пакеты: UniTask, MessagePipe, R3 (NuGet `R3` + `R3.Unity`), Input System, PrimeTween, TextMeshPro, нужный DI (Reflex / Extenject / VContainer).
2. `Project Settings → Player → Active Input Handling` = `Input System Package (New)` или `Both`.
3. `Window → TextMeshPro → Import TMP Essential Resources` (один раз на проект).
4. Built-in Render Pipeline → свойство цвета материалов `_Color`. Для URP/HDRP — `_BaseColor` (шаг 3.4).
5. Мешают старые сохранения → `Edit → Clear All PlayerPrefs`.

---

## 1. Папки и ассеты из `Examples`

`Examples` будет удалён, поэтому всё нужное из него копируем в `Content/`.

1. Создать папки:
   - `Content/Art/Textures`, `Content/Art/Materials`;
   - `Content/Input`, `Content/Definitions`, `Content/Grids`, `Content/Catalogs`;
   - `Content/Prefabs/UI`, `Content/Prefabs/World`;
   - `Content/Scenes`.
2. Скопировать (`Ctrl+D` в Project, затем перетащить копию в новую папку):

| Откуда | Куда | Используется |
|---|---|---|
| `Examples/Textures/Grid/GridBackground_2.psd` | `Content/Art/Textures` | спрайт UI-ячейки (как у старого `GridCell`) |
| `Examples/Textures/Items/Katana.psd`, `Ammo.psd`, `Pistol.psd`, `Grenade.psd`, `Money.psd` | `Content/Art/Textures` | спрайты UI-предметов |
| `Examples/Materials/GridCellMaterial.mat` | `Content/Art/Materials` | 3D-ячейка |
| `Examples/Materials/GridItemMaterial.mat` | `Content/Art/Materials` | 3D-предметы |

3. Ввод: `Assets/GridSystem/New/Samples/Input/GameInput.inputactions` — старый ассет, уже перенесён. Ничего не копировать.

---

## 2. Данные

### 2.1 Ввод — `Content/Input/PointerInputConfig`
`Create → Grid System → Pointer Input Config`.

| Поле | Значение (вложенные ассеты под `GameInput`, раскрыть стрелкой) | Binding |
|---|---|---|
| `Point` | `DragDrop/Moved` | `<Mouse>/position`, Value / Vector2 |
| `Press` | `DragDrop/LMB_Hold` | `<Mouse>/leftButton`, Hold (используются `started` / `canceled`) |
| `Rotate` | `DragDrop/Rotation` | `<Keyboard>/r` |

Действия `Control/Inventory` (I), `Control/Interaction` (E), `Control/SLWorld` (C) назначаются в сцене (шаг 5.7).

### 2.2 Предметы — `Content/Definitions/<Item>`
`Create → Grid System → Samples → Sample Item Definition`, по одному ассету на предмет.

Редактор формы:
- задать `Width` / `Height`;
- ЛКМ по клетке — занята / пуста;
- ПКМ — pivot (оранжевая «P»): точка привязки к клетке и центр поворота;
- строка 0 — верхняя.

| Ассет | Display Name | Is Rotatable | W × H | Клетки сверху вниз (`P` pivot, `x` занята, `.` пусто) | Damage | Attack Speed | Range |
|---|---|---|---|---|---|---|---|
| `Katana` | Katana | ✔ | 1 × 3 | `x` / `P` / `x` | 30 | 1.2 | 1.5 |
| `Bullet` | Bullet | ✔ | 1 × 1 | `P` | 5 | 0 | 0 |
| `Pistol` | Pistol | ✔ | 2 × 2 | `P x` / `x .` | 15 | 2 | 20 |
| `LaserTower` | Laser Tower | ✔ | 1 × 1 | `P` | 50 | 0.5 | 8 |
| `Wall` | Wall | ✔ | 2 × 2 | `P x` / `x .` | 0 | 0 | 0 |

`Id` заполняется сам (GUID ассета). Не трогать.

### 2.3 Каталог — `Content/Definitions/ItemDefinitionCatalog`
`Create → Grid System → Item Definition Catalog`. `Items` = все 5 предметов. По нему восстанавливаются сохранения.

### 2.4 Merge — `Content/Definitions/MergeRules`
`Create → Grid System → Merge Rules`, `Rules`:

| # | First | Second | Result |
|---|---|---|---|
| 0 | Katana | Katana | Bullet |
| 1 | Bullet | Bullet | Pistol |
| 2 | LaserTower | LaserTower | Wall |

Результат ставится в pivot целевого предмета. Он должен поместиться в клетки обоих ингредиентов плюс свободные клетки рядом.

### 2.5 Размеры сеток — `Content/Grids`
`Create → Grid System → Grid Definition`:

| Ассет | Width | Height | Где |
|---|---|---|---|
| `MainGrid` | 6 | 4 | `MainInventory` |
| `ChestGrid` | 4 | 4 | `OthersInventories` |
| `HandGrid` | 6 | 2 | `OutsideParent` |
| `WorldGrid` | 10 | 10 | `WorldPrefab` |

### 2.6 Настройки — `Content/GridSystemSettings`
`Create → Grid System → Settings`:
- `Items` = `ItemDefinitionCatalog`;
- `Merge Rules` = `MergeRules`;
- `Input` = `PointerInputConfig`.

---

## 3. Префабы

### 3.1 UI-ячейка — `Content/Prefabs/UI/UI_Cell`
Аналог старого `UIPrefabs/GridCell`.
```
UI_Cell        RectTransform, Image
```
- `RectTransform`: Anchors `(0.5, 0.5)–(0.5, 0.5)`, Pivot `(0.5, 0.5)`. Размер, позицию и поворот выставляет поверхность.
- `Image`: Source Image = `Art/Textures/GridBackground_2`, Color белый, `Raycast Target` выкл.

### 3.2 3D-ячейка — `Content/Prefabs/World/World_Cell`
Аналог старого `WorldPrefabs/GridItem`.
```
World_Cell     Transform
└─ Quad        MeshFilter (Quad), MeshRenderer
```
- `World_Cell`: пустой, Position / Rotation 0, Scale 1. Позицию и поворот корня перезаписывает поверхность.
- `Quad` (`3D Object → Quad`):
  - Position `(0, 0.01, 0)`, Rotation `(90, 0, 0)`, Scale `(0.95, 0.95, 1)` — зазор между клетками;
  - `MeshCollider` удалить;
  - Material = `Art/Materials/GridCellMaterial`.

### 3.3 UI-предметы — `Content/Prefabs/UI/UI_<Item>`
По одному на каждый из 5 предметов: `UI_Katana`, `UI_Bullet`, `UI_Pistol`, `UI_LaserTower`, `UI_Wall`. Аналог старых `Items/UI/*` + `ItemsVisual/*Visual`.
```
UI_Pistol      RectTransform, UIItemView, PrimeTweenItemMotion, GraphicItemHighlighter
└─ Body        RectTransform, Image
```
**`UI_Pistol`**
- `RectTransform`: Anchors `(0.5, 0.5)–(0.5, 0.5)` (НЕ stretch). Размер и pivot выставляются из формы предмета.
- `PrimeTweenItemMotion`:
  - `Target` = `UI_Pistol`;
  - `Move Duration` 0.15, `Rotate Duration` 0.1, `Ease` OutQuad (по умолчанию).
  - Без PrimeTween — `InstantItemMotion`; с define `DOTWEEN` — `DOTweenItemMotion`.
- `GraphicItemHighlighter`:
  - `Graphic` = `Body` (Image);
  - `Palette` по умолчанию: Normal белый, Valid зелёный, Invalid красный.
- `UIItemView`:
  - `Motion` = `PrimeTweenItemMotion`;
  - `Highlighter` = `GraphicItemHighlighter`;
  - `Body` = `Body`;
  - `Rect` = `UI_Pistol`.

**`Body`**
- `RectTransform`: stretch.
- `Image`: `Preserve Aspect` вкл, `Raycast Target` выкл. Спрайт рисуется в неповёрнутой ориентации формы.

| Префаб | Source Image |
|---|---|
| `UI_Katana` | `Katana` |
| `UI_Bullet` | `Ammo` |
| `UI_Pistol` | `Pistol` |
| `UI_LaserTower` | `Grenade` |
| `UI_Wall` | `Money` |

### 3.4 3D-предметы — `Content/Prefabs/World/World_<Item>`
По одному на каждый из 5 предметов. Аналог старых `Items/World/*`.
```
World_Wall     Transform, WorldItemView, PrimeTweenItemMotion, RendererItemHighlighter
└─ Model       Transform
   ├─ Cube     MeshFilter (Cube), MeshRenderer
   └─ ...
```
**Корень**
- Пустой, Position / Rotation 0, Scale 1. Коллайдеры не нужны.
- `PrimeTweenItemMotion`: `Target` = корень.
- `RendererItemHighlighter`:
  - `Renderers` = все `MeshRenderer` кубов;
  - `Color Property` = `_Color` (Built-in; по умолчанию стоит `_BaseColor` для URP/HDRP).
- `WorldItemView`:
  - `Motion` = `PrimeTweenItemMotion`;
  - `Highlighter` = `RendererItemHighlighter`;
  - `Body` = `Model`;
  - `Model` = `Model`.

**`Model`**
- Пустой. Его `localPosition` выставляется кодом: модель центрируется по форме.

**Кубы** (`3D Object → Cube`)
- `BoxCollider` удалить, Material = `Art/Materials/GridItemMaterial`.
- 1 юнит = 1 клетка. Столбцы формы идут по `+X`, строки по `-Z`, центр модели — центр габаритов формы.

| Префаб | Кубы: Position / Scale |
|---|---|
| `World_Katana` (1×3) | `(0, 0.4, 1)`, `(0, 0.4, 0)`, `(0, 0.4, -1)` / `(0.9, 0.8, 0.9)` |
| `World_Bullet` (1×1) | `(0, 0.4, 0)` / `(0.9, 0.8, 0.9)` |
| `World_Pistol` (2×2, `P x / x .`) | `(-0.5, 0.4, 0.5)`, `(0.5, 0.4, 0.5)`, `(-0.5, 0.4, -0.5)` / `(0.9, 0.8, 0.9)` |
| `World_LaserTower` (1×1) | `(0, 0.8, 0)` / `(0.9, 1.6, 0.9)` |
| `World_Wall` (2×2, `P x / x .`) | `(-0.5, 0.4, 0.5)`, `(0.5, 0.4, 0.5)`, `(-0.5, 0.4, -0.5)` / `(0.9, 0.8, 0.9)` |

### 3.5 Каталоги вьюх — `Content/Catalogs`
`Create → Grid System → Item View Catalog`:
- `UIItemViews` → `Entries`: 5 записей `Definition` → `Prefab` (`Katana` → `UI_Katana`, …).
- `WorldItemViews` → `Entries`: 5 записей (`Katana` → `World_Katana`, …).

Предмет попадает только на поверхность, в каталоге которой он есть. Для переноса UI ↔ 3D он должен быть в обоих каталогах. Результаты merge — тоже.

---

## 4. Как устроена сцена

Итоговая иерархия. Имена — как в старой сцене; в скобках — что было там.
```
SceneContext / SceneScope / LifetimeScope   ← объект DI, шаг 6 (было: SceneContext)
└─ GameInstaller                            ← только Zenject (было: GameInstaller)
GridSystem                GridSystemContext, SampleModule        (было: InteractionSystem + конфиги GameInstaller)
Environment
├─ Directional Light      Light
├─ Main Camera            Camera, AudioListener                  (PhysicsRaycaster больше не нужен)
└─ WorldPrefab            WorldGridSurface                       (было: GameObjectContext, RootWorldInstaller, WorldGridView, MeshCollider)
   ├─ Ground              MeshFilter (Plane), MeshRenderer
   │  └─ CellsParent
   └─ ItemsParent
EventSystem               EventSystem, InputSystemUIInputModule  (не обязателен, для кнопок)
WindowsSystem             Canvas, CanvasScaler, GraphicRaycaster (было: + GridLayoutGroup, GameObjectContext, RootWindowInstaller, InventoriesView, ExampleSelectableSpawnSystem, DescriptionView)
├─ OutsideParent          Image, UIGridSurface                   (было: LayoutElement, HorizontalLayoutGroup)
│  ├─ CellsParent
│  ├─ ItemsParent
│  └─ HelpWindow
│     └─ Text (Legacy)    Text
├─ OthersInventories      Image, UIGridSurface                   (было: GridLayoutGroup, ContentSizeFitter, Image, UIGridView)
│  ├─ CellsParent
│  ├─ ItemsParent
│  └─ InventoryHeader
│     └─ HeaderDescription Text
├─ MainInventory          Image, UIGridSurface                   (как OthersInventories)
│  ├─ CellsParent
│  ├─ ItemsParent
│  └─ InventoryHeader
│     └─ HeaderDescription Text
└─ InventoryItemView      ItemDescriptionView, VerticalLayoutGroup (было: LayoutElement, VerticalLayoutGroup + DescriptionView на WindowsSystem)
   ├─ BG                  Image, LayoutElement
   ├─ DataContainer       Image  → Description (TMP), Data (TMP)   "Name:"
   ├─ DataContainer (1)   Image  → Description (TMP), Data (TMP)   "Damage:"
   ├─ DataContainer (2)   Image  → Description (TMP), Data (TMP)   "Attack Speed:"
   └─ DataContainer (3)   Image  → Description (TMP), Data (TMP)   "Range:"
PoolParent                                                         (было: WindowsSystem/PoolParent и WorldPrefab/PoolParent)
```

Что исчезло и почему:
- `InteractionSystem` (3× `ExampleInteractionSystem`) → `SampleModule → Data → Windows`.
- `ExampleSelectableSpawnSystem` → `SampleModule → Data → Hand Spawn Pool / Hand Spawn Count`.
- `GameObjectContext`, `RootWindowInstaller`, `RootWorldInstaller` → один `GridSystemContext`, ставится в любой DI.
- `TempParent` → превью переноса живёт на поверхности под курсором.
- `GridLayoutGroup` / `ContentSizeFitter` / `HorizontalLayoutGroup` → размер и раскладку задаёт `UIGridSurface`. Layout-группы с ним конфликтуют.
- `MeshCollider`, `PhysicsRaycaster` → указатель переводится в клетку математически, без физики.

Порядок детей в `WindowsSystem` = порядок отрисовки: `InventoryItemView` последним, поверх окон. В старой сцене он был первым и рисовался под окнами.

---

## 5. Сцена (общая для всех DI)

`Content/Scenes` → `Create → Scene` → `GridSystem_Common`. Позже копируется под каждый DI (шаг 6).
Все объекты внутри `WindowsSystem` — слой `UI`.

### 5.1 Environment
1. `Environment` — пустой объект в корне, Transform по нулям.
2. `Directional Light` (ребёнок `Environment`): Position `(0, 0, 0)`, Rotation `(30, 0, 0)`; `Light` → Type `Directional`.
3. `Main Camera` (ребёнок `Environment`):
   - Tag `MainCamera`, Position `(0, 15, -15)`, Rotation `(45, 0, 0)`;
   - `Camera`: Projection Perspective, Field of View 60;
   - `AudioListener`.

### 5.2 WorldPrefab (3D-сетка, окно `C`)
1. `WorldPrefab` — пустой ребёнок `Environment`, Transform по нулям.
2. `Ground` — ребёнок `WorldPrefab`: `3D Object → Plane`, Transform по нулям, Scale `(1, 1, 1)`. `MeshCollider` удалить. Plane 10×10 юнитов = сетка 10×10 по 1 юниту.
3. `CellsParent` — пустой ребёнок `Ground`, Transform по нулям.
4. `ItemsParent` — пустой ребёнок `WorldPrefab`, Transform по нулям.
5. `WorldPrefab` → `WorldGridSurface`:

| Поле | Значение |
|---|---|
| `Views` | `WorldItemViews` |
| `Items Root` | `WorldPrefab/ItemsParent` |
| `Cells Root` | `WorldPrefab/Ground/CellsParent` |
| `Cell Prefab` | `World_Cell` |
| `Cell Size` | `(1, 1)` |
| `Spacing` | `(0, 0)` |
| `Priority` | `0` |
| `Camera` | `Main Camera` |

Сетка лежит в локальной плоскости XZ объекта, центр сетки — позиция объекта. Весь `WorldPrefab` — корень окна `World`: по `C` включается и выключается целиком, вместе с `Ground`.

### 5.3 WindowsSystem (Canvas)
1. `GameObject → UI → Canvas`, переименовать в `WindowsSystem`. Создастся `EventSystem` — можно оставить.
2. `Canvas`: Render Mode `Screen Space - Overlay`.
3. `CanvasScaler`:
   - UI Scale Mode `Scale With Screen Size`;
   - Reference Resolution `1920 × 1080`;
   - Screen Match Mode `Shrink` (как в старой сцене).
4. `GraphicRaycaster` — оставить.

### 5.4 Окна MainInventory / OthersInventories / OutsideParent
Размер окна выставляет `UIGridSurface`: `клеток × 84 − 4` при Cell Size 80 и Spacing 4.

| Окно | Anchors | Pivot | Pos X, Y | Размер (ставится сам) | Grid |
|---|---|---|---|---|---|
| `MainInventory` | `(0, 1)–(0, 1)` | `(0, 1)` | `60, -100` | `500 × 332` | `MainGrid` 6×4 |
| `OthersInventories` | `(0, 1)–(0, 1)` | `(0, 1)` | `60, -540` | `332 × 332` | `ChestGrid` 4×4 |
| `OutsideParent` | `(0.5, 0)–(0.5, 0)` | `(0.5, 0)` | `0, 60` | `500 × 164` | `HandGrid` 6×2 |

Для каждого окна:
1. `GameObject → UI → Image` в `WindowsSystem`, имя из таблицы, RectTransform из таблицы. Width / Height можно не трогать.
2. `Image` (фон сетки, как в старой сцене): Color `(1, 1, 1, 1)`, Source Image пусто, `Raycast Target` выкл.
3. Дети по порядку (`Create Empty` внутри UI):
   - `CellsParent` — stretch, Scale 1;
   - `ItemsParent` — stretch, Scale 1. Ниже `CellsParent`: предметы рисуются поверх ячеек.
4. `UIGridSurface` на окне:

| Поле | Значение |
|---|---|
| `Views` | `UIItemViews` |
| `Items Root` | `<окно>/ItemsParent` |
| `Cells Root` | `<окно>/CellsParent` |
| `Cell Prefab` | `UI_Cell` |
| `Cell Size` | `(80, 80)` |
| `Spacing` | `(4, 4)` |
| `Priority` | `10` |
| `Rect` | само окно |
| `Event Camera` | пусто (Overlay) |

`Priority` 10 у UI выше 0 у мира: клик по окну не уходит в 3D-сетку под ним.

**Заголовки** — `InventoryHeader` в `MainInventory` и `OthersInventories` (как старый префаб `InventoryHeader`):
- `InventoryHeader`: пустой UI, Anchors `(0, 1)–(1, 1)`, Pivot `(0.5, 0)`, Pos `(0, 4)`, Height 40.
- `HeaderDescription` (ребёнок): stretch, `Text`:
  - текст `Main Inventory` / `Other Inventory`;
  - Font Size 24, Alignment Lower Center, Color белый, `Raycast Target` выкл.

**Подсказка** — `HelpWindow` в `OutsideParent` (как старый префаб `HelpWindow`):
- `HelpWindow`: пустой UI, Anchors `(0.5, 1)–(0.5, 1)`, Pivot `(0.5, 0)`, Pos `(0, 10)`, Size `608 × 80`.
- `Text (Legacy)` (ребёнок): stretch, `Text`:
  - текст `PRESS "E" FOR OPEN CHEST` / `PRESS "I" FOR OPEN MAIN INVENTORY` / `PRESS "C" FOR LOAD/UNLOAD WORLD` (три строки);
  - Font Size 18, Alignment Middle Center, Color белый, `Raycast Target` выкл.

Видимость `MainInventory`, `OthersInventories`, `WorldPrefab` переключает код по открытию поверхности. Начальное active в сцене неважно. `OutsideParent` открыт всегда.

### 5.5 InventoryItemView (описание предмета)
Последний ребёнок `WindowsSystem`. Показывается при наведении на предмет, иначе скрыт.

1. **`InventoryItemView`**:
   - пустой UI: Anchors `(1, 0.5)–(1, 0.5)`, Pivot `(1, 0.5)`, Pos `(-60, 0)`, Size `500 × 300`;
   - `VerticalLayoutGroup`:
     - Padding 20 со всех сторон, Spacing 10, Child Alignment `Middle Center`;
     - Control Child Size: Width ✔, Height ✔;
     - Child Force Expand: Width ✔, Height ✔.
2. **`BG`** (первый ребёнок):
   - stretch;
   - `Image`: Color `(0.464, 0.464, 0.464, 1)`, `Raycast Target` выкл;
   - `LayoutElement`: `Ignore Layout` ✔.
3. **`DataContainer`, `DataContainer (1)`, `DataContainer (2)`, `DataContainer (3)`** — пустые UI. Размер задаёт VerticalLayoutGroup.
   - `Image`: Color белый, `Raycast Target` выкл.
   - Дети каждой строки:
     - `Description` (подпись): Anchors `(0, 0)–(0.5, 1)`, offsets 0. `TextMeshPro - Text (UI)`: текст из таблицы, Font Size 24, Alignment Left / Middle, Color `(0.196, 0.196, 0.196, 1)`, `Raycast Target` выкл.
     - `Data` (значение): Anchors `(0.5, 0)–(1, 1)`, offsets 0. `TextMeshPro - Text (UI)`: те же настройки, текст `info` (заменится кодом).

| Строка | `Description` |
|---|---|
| `DataContainer` | `Name:` |
| `DataContainer (1)` | `Damage:` |
| `DataContainer (2)` | `Attack Speed:` |
| `DataContainer (3)` | `Range:` |

4. **`ItemDescriptionView`** на `InventoryItemView`:

| Поле | Значение |
|---|---|
| `Root` | `InventoryItemView` (сам объект) |
| `Name` | `DataContainer/Data` |
| `Damage` | `DataContainer (1)/Data` |
| `Attack Speed` | `DataContainer (2)/Data` |
| `Range` | `DataContainer (3)/Data` |

Старые Legacy `Text` здесь не подходят: `ItemDescriptionView` пишет числа в TMP без аллокаций.

### 5.6 PoolParent
Пустой объект в корне сцены, Transform по нулям. Сюда уходят выключенные вьюхи предметов.

### 5.7 GridSystem
Пустой объект в корне, Transform по нулям, компоненты `GridSystemContext` и `SampleModule`.

**`GridSystemContext`**

| Поле | Значение |
|---|---|
| `Settings` | `GridSystemSettings` |
| `Pool Root` | `PoolParent` |
| `Modules` | 1 элемент: `SampleModule` с этого же объекта |

**`SampleModule`**
- `Description` = `WindowsSystem/InventoryItemView`.
- `Data → Hand` (рука):
  - `Key` = `Hand`;
  - `Definition` = `HandGrid`;
  - `Surface` = `WindowsSystem/OutsideParent`.
- `Data → Hand Spawn Pool` = 3 элемента: `Katana`, `Bullet`, `Pistol`.
- `Data → Hand Spawn Count` = `3`. Только при первом запуске, без повторов.
- `Data → Windows` = 3 элемента:

| # | Inventory → Key | Inventory → Definition | Inventory → Surface | Root | Toggle | Is Opened On Start |
|---|---|---|---|---|---|---|
| 0 | `Main` | `MainGrid` | `MainInventory` | `MainInventory` | `Control/Inventory` (I) | ✘ |
| 1 | `Chest` | `ChestGrid` | `OthersInventories` | `OthersInventories` | `Control/Interaction` (E) | ✘ |
| 2 | `World` | `WorldGrid` | `WorldPrefab` | `WorldPrefab` | `Control/SLWorld` (C) | ✘ |

`Start Items` (Definition / Origin / Rotation):
- `Windows[0]` (Main) — пусто.
- `Windows[1]` (Chest) — `Katana (0, 1) 0`, `Katana (1, 1) 0`, `Bullet (3, 0) 0`.
- `Windows[2]` (World) — `LaserTower (2, 2) 0`, `Wall (5, 5) 0`, `LaserTower (7, 3) 0`.

`Origin` — клетка pivot предмета (x — столбец, y — строка сверху). `Start Items` ставятся только когда нет сохранения. Окно сохраняется при закрытии и при выходе из Play Mode. `Toggle` — вложенные `InputActionReference` под `GameInput`.

Сохранить сцену.

---

## 6. DI

Скопировать `GridSystem_Common` (`Ctrl+D`) в `GridSystem_Reflex`, `GridSystem_Zenject`, `GridSystem_VContainer`. В каждой — ровно один объект DI. Всё остальное одинаково.

### 6.1 Zenject — `GridSystem_Zenject` (как в старой сцене)
```
SceneContext       SceneContext
└─ GameInstaller   ZenjectGridInstaller
```
1. `GameObject → Zenject → Scene Context` → `SceneContext`.
2. Дочерний пустой `GameInstaller`, на него `ZenjectGridInstaller`: `Context` = `GridSystem`.
3. `SceneContext`:
   - `Mono Installers` = [`GameInstaller`];
   - `Scriptable Object Installers`, `Installer Prefabs` пусто;
   - `Parent Contract Names` пусто.
4. `ProjectContext` не нужен.

### 6.2 Reflex — `GridSystem_Reflex`
```
SceneScope         ContainerScope, ReflexGridInstaller
```
1. Один раз на проект: `Assets/Resources/ReflexSettings` (строго в корне `Resources`). Уже есть в проекте.
2. `GameObject → Reflex → SceneScope` (объект с `ContainerScope`).
3. На него `ReflexGridInstaller`: `Context` = `GridSystem`.

### 6.3 VContainer — `GridSystem_VContainer`
```
LifetimeScope      GridSystemLifetimeScope
```
1. Пустой `LifetimeScope` в корне, на него `GridSystemLifetimeScope`.
2. `Context` = `GridSystem`, `Parent` пусто, `Auto Run` ✔.

Свой scope вместо готовых компонентов:
- Reflex: `containerBuilder.InstallGridSystem(context)` в своём `IInstaller`;
- Zenject: `Container.InstallGridSystem(context)` в своём `MonoInstaller`;
- VContainer: `builder.InstallGridSystem(context)` в `Configure` своего `LifetimeScope`.

---

## 7. Если начинаешь с копии старой сцены

Скопировать `Examples/Scenes/TestInventoryScene` в `Content/Scenes/GridSystem_Zenject`.

Подготовка:
1. Инстансы `WindowsSystem` и `WorldPrefab` ссылаются на префабы `Examples`. Сделать `Prefab → Unpack Completely` для обоих, иначе после удаления `Examples` объекты пропадут.
2. Вложенные `InventoryHeader` и `HelpWindow` распакуются вместе с ними.

Дальше пройти по таблице: «удалить» — Remove Component / Delete, «добавить» — по разделу 5.

| Объект | Удалить | Добавить / изменить |
|---|---|---|
| `InteractionSystem` | объект целиком | — (5.7 `Windows`) |
| `SceneContext` | — | `Mono Installers` = [`GameInstaller`] (6.1) |
| `SceneContext/GameInstaller` | `GameInstaller` | `ZenjectGridInstaller` (6.1) |
| `Environment/Main Camera` | `PhysicsRaycaster` | — |
| `Environment/WorldPrefab` | `GameObjectContext`, `RootWorldInstaller`, `WorldGridView`, `MeshCollider` | `WorldGridSurface` (5.2) |
| `WorldPrefab/Ground` | — | проверить `MeshCollider` (удалить, если есть) |
| `WorldPrefab/PoolParent` | объект | — |
| `WorldPrefab` | — | ребёнок `ItemsParent` (5.2) |
| `WindowsSystem` | `GridLayoutGroup`, `GameObjectContext`, `RootWindowInstaller`, `InventoriesView`, `ExampleSelectableSpawnSystem`, `DescriptionView` | `CanvasScaler` проверить (5.3) |
| `MainInventory`, `OthersInventories` | `UIGridView`, `GridLayoutGroup`, `ContentSizeFitter` | ребёнок `CellsParent` над `ItemsParent`, `UIGridSurface`, RectTransform (5.4) |
| `InventoryHeader` (оба) | `LayoutElement` | RectTransform (5.4) |
| `OutsideParent` | `LayoutElement`, `HorizontalLayoutGroup` | `Image`, дети `CellsParent` / `ItemsParent`, `UIGridSurface`, RectTransform (5.4) |
| `HelpWindow` | `LayoutElement` | RectTransform (5.4) |
| `TempParent` | объект | — |
| `WindowsSystem/PoolParent` | объект | `PoolParent` в корне сцены (5.6) |
| `InventoryItemView` | `LayoutElement` | сделать active, переместить последним ребёнком, `ItemDescriptionView`, RectTransform и VerticalLayoutGroup (5.5) |
| `InventoryItemView/BG` | — | переместить первым ребёнком |
| `DataContainer/Description`, `DataContainer/Data` | `Text` | `TextMeshPro - Text (UI)` (5.5) |
| `DataContainer` | — | дублировать до 4 строк (5.5) |
| корень сцены | — | `GridSystem` (5.7), `PoolParent` (5.6) |
| `EventSystem` | — | оставить |

После всех замен:
- в Inspector не должно остаться `Missing (Mono Script)`;
- не должно остаться компонентов из `Examples`.

Затем копии под Reflex / VContainer — по шагу 6.

---

## 8. Проверка в Play Mode
1. Консоль без ошибок. Внизу рука `OutsideParent` с 3 разными предметами, остальные окна закрыты.
2. Перенос внутри руки:
   - превью зелёное — можно поставить, красное — нельзя;
   - `R` во время переноса — поворот.
3. `I` — открыть `MainInventory`. Перенести предмет из руки и обратно.
4. `E` — открыть сундук: две Katana и Bullet. Перенести в руку или Main.
5. `C` — показать 3D-сетку с башнями и стеной. Перенести предмет из UI на сетку и обратно: над сеткой превью становится 3D-моделью.
6. Merge:
   - Katana на Katana → Bullet;
   - Bullet на Bullet → Pistol;
   - LaserTower на LaserTower → Wall.
7. Навести на предмет — справа `InventoryItemView` с параметрами. Увести — скрывается.
8. Закрыть окно или выйти из Play Mode, снова запустить — раскладка восстановлена.

## 9. Частые ошибки
| Симптом | Причина |
|---|---|
| `KeyNotFoundException` в `ItemViewCatalog` | Предмет или результат merge не добавлен в каталог вьюх поверхности, на которой он появляется |
| Мышь ни на что не реагирует | В `PointerInputConfig` не назначены действия; `Active Input Handling` без Input System |
| `I` / `E` / `C` не работают | В `Windows[i] → Toggle` не назначено действие `Control/...` |
| Окно не появляется | `Root` не тот объект, или `Surface` указывает на другое окно |
| Ячейки разъезжаются, окно не того размера | На окне или на `WindowsSystem` остались `GridLayoutGroup` / `ContentSizeFitter` / `HorizontalLayoutGroup` |
| Предметы под ячейками | `ItemsParent` выше `CellsParent` в иерархии |
| Описание пустое | Поля `ItemDescriptionView` указывают на `Description`, а не на `Data` |
| Описание под окнами | `InventoryItemView` не последний ребёнок `WindowsSystem` |
| UI-предмет смещён относительно клеток | У корня UI-префаба anchors stretch, или `Body` не растянут |
| 3D-модель смещена | Кубы не по таблице 3.4, или масштаб не 1 юнит на клетку |
| 3D-ячейки стоят вертикально | Поворот `(90, 0, 0)` задан корню `World_Cell`, а не `Quad` |
| 3D-предмет не подсвечивается | `Color Property` не `_Color` (Built-in) |
| Клик по окну попадает в 3D-сетку | `Priority` UI-поверхности не больше, чем у `WorldPrefab` |
| `NullReferenceException` при старте | Пустое поле в `GridSystemContext`, `SampleModule`, поверхности или префабе |
| Объекты пропали после удаления `Examples` | Инстансы не распакованы (шаг 7.1) или ассеты не скопированы (шаг 1) |
