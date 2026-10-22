# Ручная настройка сцены примера

Инструкция собирает демо-сцену v2: UI-инвентарь (Main) с открытием по клавише, UI-«рука» (Hand) со стартовыми предметами, 3D-сетка (World), drag&drop между всеми тремя (включая UI ↔ 3D), поворот, merge, тултип, сохранения.
Шаги 0–5 общие для всех DI, шаг 6 — отдельно для Reflex, Zenject и VContainer. Каждая DI-сцена — копия общей сцены с другим объектом DI.

Все новые ассеты кладём в `Assets/GridSystem/New/Samples/Content/` (дальше — `Content/`).

> **Сцена в репозитории собрана не по этой инструкции.** `Examples/Scenes/TestInventoryScene_Zenject.unity`
> воспроизводит сцену v1 один в один и поэтому переиспользует её префаб `WindowsSystem` и другие числа:
> рука 5×1 (лента) с 4 предметами, клетка 100 px, инвентарь и сундук 3×3, тултип в две строки.
> Инструкция ниже собирает независимое демо автора v2 — она о том, **как связывать систему**,
> а не о том, что лежит в репозитории.

---

## 0. Предусловия

1. Установлены: UniTask, MessagePipe, R3 (NuGet `R3` + `R3.Unity`), Input System, PrimeTween, нужный DI.
2. `Project Settings → Player → Active Input Handling` = `Input System Package (New)` или `Both`.
3. Проект на Built-in Render Pipeline (`GraphicsSettings → Custom Render Pipeline` пусто) → у 3D-подсветки свойство цвета `_Color`. Для URP/HDRP — `_BaseColor`.
4. Если сохранения от прошлых запусков мешают: `Edit → Clear All PlayerPrefs`.

---

## 1. Ввод

1. `Content/Input` → ПКМ → `Create → Input Actions` → `GridSystemInput`.
2. Открыть, создать Action Map `Grid`:

| Action | Action Type | Control Type | Binding |
|---|---|---|---|
| `Point` | Value | Vector2 | `<Pointer>/position` |
| `Press` | Button | — | `<Mouse>/leftButton`, `<Touchscreen>/primaryTouch/press` |
| `Rotate` | Button | — | `<Keyboard>/r`, `<Mouse>/rightButton` |
| `ToggleInventory` | Button | — | `<Keyboard>/i` |

3. `Save Asset`. Под ассетом появятся вложенные `InputActionReference` для каждого действия.
4. `Content/Input` → `Create → Grid System → Pointer Input Config` → `PointerInputConfig`:
   - `Point` = `Grid/Point`, `Press` = `Grid/Press`, `Rotate` = `Grid/Rotate`.

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
| `HandGrid` | 6 | 2 |
| `WorldGrid` | 10 | 10 |

Инвентарь высотой **1 ряд — это лента** (hotbar), а не сетка:

- предмет занимает по одной клетке на каждую свою колонку, ряды формы игнорируются,
  поэтому в один ряд помещается предмет любой высоты;
- вьюха центрируется по собственным габаритам, а не по пивот-клетке, поэтому предметы
  разной высоты стоят на одной линии и не смещаются при добавлении новых форм.

Отдельного флага нет: признак выводится из `Height == 1` (`GridInventory.IsBar`, `GridSurface.IsBar`).
Сетка из двух и более рядов ведёт себя как обычно.

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

`Content/Scenes` → новая сцена `GridSystem_Common` (потом копируется под каждый DI).

### 4.1 Камера и свет
1. `Main Camera`: Position `(0, 12, -9)`, Rotation `(55, 0, 0)`.
2. `Directional Light` по умолчанию.

### 4.2 3D-сетка
```
WorldGrid            WorldGridSurface         Position (0, 0, 0)
├─ Cells
└─ Items
```
`WorldGridSurface`: `Views` = `WorldItemViews`, `Items Root` = `Items`, `Cells Root` = `Cells`, `Cell Prefab` = `World_Cell`, `Cell Size` = `(1, 1)`, `Spacing` = `(0.05, 0.05)`, `Priority` = `0`, `Camera` = `Main Camera`.
Сетка лежит в локальной плоскости XZ объекта, объект — её центр.

### 4.3 Canvas
```
Canvas               Canvas (Screen Space - Overlay), CanvasScaler (Scale With Screen Size 1920×1080, Match 0.5)
├─ MainWindow        Image (фон), anchors center, Size (560, 420), Position (0, 60)   ← неактивен на старте не обязательно
│  └─ MainGrid       UIGridSurface, anchors center, pivot center, Position (0, -20)
│     ├─ Cells       anchors stretch, offsets 0
│     └─ Items       anchors stretch, offsets 0
├─ HandPanel         Image (фон), anchors bottom-center, Size (560, 200), Position (0, 120)
│  └─ HandGrid       UIGridSurface, anchors center, pivot center
│     ├─ Cells
│     └─ Items
└─ Tooltip           ItemTooltipView                                                   ← последний ребёнок Canvas
   └─ Panel          Image (фон, Raycast Target выкл), pivot (0, 1), Size (220, 130)
      ├─ Name        TextMeshPro - Text (UI)
      ├─ Damage      TextMeshPro - Text (UI)
      ├─ AttackSpeed TextMeshPro - Text (UI)
      └─ Range       TextMeshPro - Text (UI)
```
1. `Cells` выше `Items` в иерархии — предметы рисуются поверх ячеек. Scale у `Cells`/`Items` = 1.
2. `MainGrid` → `UIGridSurface`: `Rect` = `MainGrid`, `Event Camera` = пусто (Overlay), `Views` = `UIItemViews`, `Items Root` = `MainGrid/Items`, `Cells Root` = `MainGrid/Cells`, `Cell Prefab` = `UI_Cell`, `Cell Size` = `(80, 80)`, `Spacing` = `(4, 4)`, `Priority` = `10`.
3. `HandGrid` → `UIGridSurface`: то же, но свои `Items`/`Cells`, `Priority` = `10`.
4. Размер `MainGrid`/`HandGrid` выставляется сам по размеру сетки.
5. `Tooltip` → `ItemTooltipView`: `Root` = `Panel`, `Panel` = `Panel`, `Name`/`Damage` = тексты значений (не подписей).
6. EventSystem для работы плагина не нужен (пикинг математический, без рейкастов). Нужен только если в сцене есть кнопки.

`UIGridSurface` с priority 10 перекрывает 3D-сетку под UI: клик по инвентарю не попадёт в мир.

### 4.4 Пул
Пустой объект `Pool` в корне, выключенный (`SetActive(false)`). Сюда возвращаются вьюхи.

### 4.5 Grid System
```
GridSystem           GridSystemContext, SampleModule
```
1. `GridSystemContext`: `Settings` = `GridSystemSettings`, `Pool Root` = `Pool`, `Modules` = [`SampleModule` с этого же объекта].
2. `SampleModule`: `Config` = `SampleSceneConfig`, `Camera` = камера сцены. Больше на сцене ничего нет: UI и 3D-сетка
   создаются из префабов конфига через контейнер, поэтому три DI-сцены отличаются только установщиком.
3. `SampleSceneConfig` (ассет `Create → Grid System → Sample Scene Config`):
   - `Ui Prefab` = `SampleWindowsSystem`, `World Prefab` = `SampleWorldGrid`.
   - `Main` / `Chest` / `Hand` / `World` — по инвентарю: `Key` (ключ сохранения, уникальный),
     `Definition` (размер сетки) и `Start Items` (`Definition + Origin + Rotation`), которые
     раскладываются только при первом запуске, когда сохранения ещё нет.
   - `Toggle Main` / `Toggle Chest` / `Toggle World` — действия открытия поверхностей.

Поверхности, окна и тултип отдаёт `SampleSceneRoot` на UI-префабе. Окна включаются/выключаются сами по открытию
поверхности, начальное состояние в префабе неважно.

Инспектор `SampleSceneConfig` и `GridSystemSettings` сам проверяет конфигурацию и показывает дефекты списком —
см. раздел 8.

---

## 5. Сохранить общую сцену
Дальше сцену копируем (`Ctrl+D` в Project) в три: `TestInventoryScene_Zenject`, `TestInventoryScene_Reflex`,
`TestInventoryScene_VContainer`. В каждой — ровно один DI из шага 6, остальные компоненты DI снять.
Две и более корневые точки DI в одной сцене дадут двойную регистрацию.

Содержимое сцены при этом не копируется: UI и 3D-сетка спавнятся из префабов конфига. Все три сцены — это
`EventSystem`, `Environment` (камера и свет) и один объект DI, на дочернем объекте которого висят
`GridSystemContext` + `SampleModule`, а под ним пустой выключенный `Pool`.

---

## 6. DI

### 6.1 Reflex — `TestInventoryScene_Reflex`
1. Один раз на проект: создать `Assets/Resources`, в нём `Create → Reflex → Settings` (`ReflexSettings` строго в корне `Resources`). `Root Scopes` можно оставить пустым.
2. В сцене: `GameObject → Reflex → SceneScope` — корневой объект с `ContainerScope`.
3. На `SceneScope` (или его дочерний объект) добавить `ReflexGridInstaller`, `Context` = объект `GridSystem`.

### 6.2 Zenject — `TestInventoryScene_Zenject`
1. `GameObject → Zenject → Scene Context`.
2. На `SceneContext` добавить `ZenjectGridInstaller`, `Context` = `GridSystem`.
3. `SceneContext → Mono Installers` → `+` → перетащить `ZenjectGridInstaller`.
4. `ProjectContext` не нужен.

### 6.3 VContainer — `TestInventoryScene_VContainer`
1. Пустой объект `LifetimeScope` в корне, добавить `GridSystemLifetimeScope`.
2. `Context` = `GridSystem`, `Parent` пусто, `Auto Run` вкл.

Свой scope вместо готовых компонентов:
- Reflex: `containerBuilder.InstallGridSystem(context)` в своём `IInstaller`.
- Zenject: `Container.InstallGridSystem(context)` в своём `MonoInstaller`.
- VContainer: `builder.InstallGridSystem(context)` в `Configure` своего `LifetimeScope`.

---

## 7. Проверка в Play Mode
1. В руке 3 разных предмета, на 3D-сетке башни и стена.
2. Перетаскивание внутри руки: превью зелёное — можно, красное — нельзя. `R`/ПКМ во время drag — поворот.
3. `I` — открыть Main, перенести предмет из руки в Main и обратно.
4. Перенести предмет из UI на 3D-сетку: над сеткой превью становится 3D-моделью, после отпускания предмет в мире. И обратно.
5. Бросить Katana на Katana → Bullet; Bullet на Bullet → Pistol; LaserTower на LaserTower → Wall.
6. Навести на предмет — тултип с параметрами.
7. Закрыть Main (`I`) или выйти из Play Mode, снова запустить — раскладка восстановлена.

## 8. Частые ошибки

Большинство из них видно до запуска: инспекторы `SampleSceneConfig` и `GridSystemSettings` показывают блок
`Validation` со списком дефектов. Проверка идёт при открытии инспектора и после каждой правки; клик по сообщению
подсвечивает виновный ассет в Project. Красное — упадёт или потеряет данные, жёлтое — работает, но, возможно, не то,
что задумано. Проверяется:

| Что | Чем это кончится в рантайме |
|---|---|
| Стартовый предмет отсутствует в каталоге вьюх своей поверхности | `KeyNotFoundException` при открытии поверхности, то есть на старте |
| Стартовый предмет не влезает в сетку или налезает на другой | `TryAdd` молча вернёт false, предмета просто не будет |
| Два инвентаря с одинаковым `Key` | Сохранения перетирают друг друга |
| Поверхность UI не приоритетнее мировой | Бросок над UI может уйти в 3D-мир |
| Запись каталога без определения или без префаба | Пустая клетка или падение при показе |
| Предмет из `ItemDefinitionCatalog` не попал ни в один каталог вьюх | Его нельзя показать нигде — мёртвая запись. Один каталог из двух — норма: именно так поверхность отказывает чужим предметам |
| Два определения с одним `Id` | Сохранение восстановит не тот предмет |
| Одна пара даёт два разных результата merge | Побеждает последнее правило |
| Ингредиент или результат merge не лежит в `ItemDefinitionCatalog` | Сохранение инвентаря с этим предметом будет сброшено при загрузке |
| Результат merge недостижим в каталоге вьюх (жёлтое) | Merge на этой поверхности будет отклонён |

Проверка стартовых раскладок прогоняется через настоящий `GridInventory.TryAdd`, а не через копию правил, поэтому
не может разойтись с рантаймом — включая правило бара для сеток высотой 1.

| Симптом | Причина |
|---|---|
| `KeyNotFoundException` в `ItemViewCatalog.GetPrefab` | Предмет (или результат merge) не добавлен в каталог вьюх поверхности, на которой он появляется. Текст исключения называет каталог и предмет |
| `InvalidOperationException` в `ItemViewCatalog.GetPrefab` | Запись в каталоге есть, но поле `Prefab` пустое |
| Ничего не реагирует на мышь | В `PointerInputConfig` не назначены действия, `Point` не `Value/Vector2`, `Active Input Handling` без Input System |
| UI-предмет смещён относительно клеток | У корня вьюхи anchors stretch, или `Body` не растянут на весь корень |
| 3D-модель смещена | Модель не отцентрована по габаритам формы или масштаб не 1 юнит на клетку |
| 3D-ячейки стоят вертикально | Поворот `(90, 0, 0)` задан корню `World_Cell`, а не дочернему Quad |
| 3D-предмет не подсвечивается | Неверный `Color Property` (`_Color` для Built-in, `_BaseColor` для URP/HDRP) |
| UI-предметы под ячейками | `Items` выше `Cells` в иерархии |
| Предмет не переносится на 3D-сетку | Его нет в `WorldItemViews` |
| `NullReferenceException` при старте | Не заполнено поле в `GridSystemContext`, `SampleModule` или поверхности |
