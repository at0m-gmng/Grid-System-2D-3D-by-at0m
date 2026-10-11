namespace GridSystem.Samples.Editor
{
    using Runtime.Composition;
    using Runtime.Surfaces;
    using TMPro;
    using Tooltip;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Creates DI independent scene objects: camera, light, surfaces, windows, tooltip, pool and grid system context.
    /// </summary>
    public sealed class SampleSceneBuilder
    {
        private const string CAMERA_NAME = "Main Camera";
        private const string LIGHT_NAME = "Directional Light";
        private const string WORLD_GRID_NAME = "WorldGrid";
        private const string CANVAS_NAME = "Canvas";
        private const string MAIN_WINDOW_NAME = "MainWindow";
        private const string MAIN_GRID_NAME = "MainGrid";
        private const string HAND_PANEL_NAME = "HandPanel";
        private const string HAND_GRID_NAME = "HandGrid";
        private const string CELLS_NAME = "Cells";
        private const string ITEMS_NAME = "Items";
        private const string TOOLTIP_NAME = "Tooltip";
        private const string PANEL_NAME = "Panel";
        private const string NAME_TEXT = "Name";
        private const string DAMAGE_TEXT = "Damage";
        private const string ATTACK_SPEED_TEXT = "AttackSpeed";
        private const string RANGE_TEXT = "Range";
        private const string POOL_NAME = "Pool";
        private const string GRID_SYSTEM_NAME = "GridSystem";
        private const string VIEWS_FIELD = "_views";
        private const string ITEMS_ROOT_FIELD = "_itemsRoot";
        private const string CELLS_ROOT_FIELD = "_cellsRoot";
        private const string CELL_PREFAB_FIELD = "_cellPrefab";
        private const string CELL_SIZE_FIELD = "_cellSize";
        private const string SPACING_FIELD = "_spacing";
        private const string PRIORITY_FIELD = "_priority";
        private const string CAMERA_FIELD = "_camera";
        private const string RECT_FIELD = "_rect";
        private const string ROOT_FIELD = "_root";
        private const string PANEL_FIELD = "_panel";
        private const string NAME_FIELD = "_name";
        private const string DAMAGE_FIELD = "_damage";
        private const string ATTACK_SPEED_FIELD = "_attackSpeed";
        private const string RANGE_FIELD = "_range";
        private const string SETTINGS_FIELD = "_settings";
        private const string POOL_ROOT_FIELD = "_poolRoot";
        private const string MODULES_FIELD = "_modules";
        private const string TOOLTIP_FIELD = "_tooltip";
        private const string MAIN_KEY_FIELD = "_data._main._key";
        private const string MAIN_DEFINITION_FIELD = "_data._main._definition";
        private const string MAIN_SURFACE_FIELD = "_data._main._surface";
        private const string HAND_KEY_FIELD = "_data._hand._key";
        private const string HAND_DEFINITION_FIELD = "_data._hand._definition";
        private const string HAND_SURFACE_FIELD = "_data._hand._surface";
        private const string WORLD_KEY_FIELD = "_data._world._key";
        private const string WORLD_DEFINITION_FIELD = "_data._world._definition";
        private const string WORLD_SURFACE_FIELD = "_data._world._surface";
        private const string MAIN_WINDOW_FIELD = "_data._mainWindow";
        private const string TOGGLE_FIELD = "_data._toggleMainWindow";
        private const string WINDOW_OPENED_FIELD = "_data._isMainWindowOpened";
        private const string START_ITEMS_FIELD = "_data._worldStartItems";
        private const string SPAWN_POOL_FIELD = "_data._handSpawnPool";
        private const string SPAWN_COUNT_FIELD = "_data._handSpawnCount";
        private const string START_DEFINITION_FIELD = "Definition";
        private const string START_ORIGIN_FIELD = "Origin";
        private const string START_ROTATION_FIELD = "Rotation";
        private const int START_ROTATION = 0;

        /// <summary>
        /// Builds scene objects in the active scene and returns the context to install into a container.
        /// </summary>
        public GridSystemContext Build(SampleContent content)
        {
            Camera camera = CreateCamera();
            CreateLight();
            WorldGridSurface world = CreateWorldGrid(content, camera);
            int uiLayer = LayerMask.NameToLayer(SampleLayout.UI_LAYER);
            RectTransform canvas = CreateCanvas(uiLayer);
            RectTransform mainWindow = CreatePanel(MAIN_WINDOW_NAME, canvas, uiLayer, SampleLayout.Center, SampleLayout.MainWindowSize,
                SampleLayout.MainWindowPosition, SampleLayout.WindowColor);
            UIGridSurface main = CreateUIGrid(MAIN_GRID_NAME, mainWindow, uiLayer, SampleLayout.MainGridPosition, content);
            RectTransform handPanel = CreatePanel(HAND_PANEL_NAME, canvas, uiLayer, SampleLayout.BottomCenter, SampleLayout.HandPanelSize,
                SampleLayout.HandPanelPosition, SampleLayout.WindowColor);
            UIGridSurface hand = CreateUIGrid(HAND_GRID_NAME, handPanel, uiLayer, Vector2.zero, content);
            ItemTooltipView tooltip = CreateTooltip(canvas, uiLayer);
            GameObject pool = new GameObject(POOL_NAME);
            pool.SetActive(false);
            return CreateGridSystem(content, pool.transform, mainWindow.gameObject, main, hand, world, tooltip);
        }

        private static Camera CreateCamera()
        {
            GameObject target = new GameObject(CAMERA_NAME);
            target.tag = SampleLayout.MAIN_CAMERA_TAG;
            target.transform.SetPositionAndRotation(SampleLayout.CameraPosition, Quaternion.Euler(SampleLayout.CameraRotation));
            target.AddComponent<AudioListener>();
            return target.AddComponent<Camera>();
        }

        private static void CreateLight()
        {
            GameObject target = new GameObject(LIGHT_NAME);
            target.transform.rotation = Quaternion.Euler(SampleLayout.LightRotation);
            target.AddComponent<Light>().type = LightType.Directional;
        }

        private static WorldGridSurface CreateWorldGrid(SampleContent content, Camera camera)
        {
            GameObject target = new GameObject(WORLD_GRID_NAME);
            Transform cells = CreateChild(CELLS_NAME, target.transform);
            Transform items = CreateChild(ITEMS_NAME, target.transform);
            WorldGridSurface surface = target.AddComponent<WorldGridSurface>();
            new SerializedObject(surface)
                .SetObject(VIEWS_FIELD, content.WorldViews)
                .SetObject(ITEMS_ROOT_FIELD, items)
                .SetObject(CELLS_ROOT_FIELD, cells)
                .SetObject(CELL_PREFAB_FIELD, content.WorldCell)
                .SetVector2(CELL_SIZE_FIELD, SampleLayout.WorldCellSize)
                .SetVector2(SPACING_FIELD, SampleLayout.WorldSpacing)
                .SetInt(PRIORITY_FIELD, SampleLayout.WORLD_PRIORITY)
                .SetObject(CAMERA_FIELD, camera)
                .Apply();
            return surface;
        }

        private static RectTransform CreateCanvas(int layer)
        {
            GameObject target = new GameObject(CANVAS_NAME, typeof(RectTransform));
            target.layer = layer;
            target.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = target.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = SampleLayout.ReferenceResolution;
            scaler.matchWidthOrHeight = SampleLayout.SCALER_MATCH;
            return (RectTransform)target.transform;
        }

        private static RectTransform CreatePanel(string name, Transform parent, int layer, Vector2 anchor, Vector2 size, Vector2 position, Color color)
        {
            RectTransform rect = CreateUI(name, parent, layer);
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return rect;
        }

        private static UIGridSurface CreateUIGrid(string name, Transform parent, int layer, Vector2 position, SampleContent content)
        {
            RectTransform rect = CreateUI(name, parent, layer);
            rect.anchorMin = SampleLayout.Center;
            rect.anchorMax = SampleLayout.Center;
            rect.pivot = SampleLayout.Center;
            rect.anchoredPosition = position;
            RectTransform cells = CreateStretchedUI(CELLS_NAME, rect, layer);
            RectTransform items = CreateStretchedUI(ITEMS_NAME, rect, layer);
            UIGridSurface surface = rect.gameObject.AddComponent<UIGridSurface>();
            new SerializedObject(surface)
                .SetObject(VIEWS_FIELD, content.UIViews)
                .SetObject(ITEMS_ROOT_FIELD, items)
                .SetObject(CELLS_ROOT_FIELD, cells)
                .SetObject(CELL_PREFAB_FIELD, content.UICell)
                .SetVector2(CELL_SIZE_FIELD, SampleLayout.UICellSize)
                .SetVector2(SPACING_FIELD, SampleLayout.UISpacing)
                .SetInt(PRIORITY_FIELD, SampleLayout.UI_PRIORITY)
                .SetObject(RECT_FIELD, rect)
                .Apply();
            return surface;
        }

        private static ItemTooltipView CreateTooltip(Transform canvas, int layer)
        {
            RectTransform root = CreateStretchedUI(TOOLTIP_NAME, canvas, layer);
            RectTransform panel = CreateUI(PANEL_NAME, root, layer);
            panel.pivot = SampleLayout.TopLeft;
            panel.sizeDelta = SampleLayout.TooltipSize;
            Image background = panel.gameObject.AddComponent<Image>();
            background.color = SampleLayout.TooltipColor;
            background.raycastTarget = false;
            VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(SampleLayout.TOOLTIP_PADDING, SampleLayout.TOOLTIP_PADDING, SampleLayout.TOOLTIP_PADDING,
                SampleLayout.TOOLTIP_PADDING);
            ItemTooltipView view = root.gameObject.AddComponent<ItemTooltipView>();
            new SerializedObject(view)
                .SetObject(ROOT_FIELD, panel.gameObject)
                .SetObject(PANEL_FIELD, panel)
                .SetObject(NAME_FIELD, CreateText(NAME_TEXT, panel, layer))
                .SetObject(DAMAGE_FIELD, CreateText(DAMAGE_TEXT, panel, layer))
                .SetObject(ATTACK_SPEED_FIELD, CreateText(ATTACK_SPEED_TEXT, panel, layer))
                .SetObject(RANGE_FIELD, CreateText(RANGE_TEXT, panel, layer))
                .Apply();
            return view;
        }

        private static TMP_Text CreateText(string name, Transform parent, int layer)
        {
            TextMeshProUGUI text = CreateUI(name, parent, layer).gameObject.AddComponent<TextMeshProUGUI>();
            text.fontSize = SampleLayout.TOOLTIP_FONT_SIZE;
            text.raycastTarget = false;
            return text;
        }

        private static GridSystemContext CreateGridSystem(SampleContent content, Transform pool, GameObject mainWindow, GridSurface main, GridSurface hand,
            GridSurface world, ItemTooltipView tooltip)
        {
            GameObject target = new GameObject(GRID_SYSTEM_NAME);
            GridSystemContext context = target.AddComponent<GridSystemContext>();
            SampleModule module = target.AddComponent<SampleModule>();
            new SerializedObject(context)
                .SetObject(SETTINGS_FIELD, content.Settings)
                .SetObject(POOL_ROOT_FIELD, pool)
                .SetObjects(MODULES_FIELD, new GridModule[] { module })
                .Apply();
            SerializedObject serialized = new SerializedObject(module)
                .SetObject(TOOLTIP_FIELD, tooltip)
                .SetString(MAIN_KEY_FIELD, SampleSpecs.MAIN)
                .SetObject(MAIN_DEFINITION_FIELD, content.MainGrid)
                .SetObject(MAIN_SURFACE_FIELD, main)
                .SetString(HAND_KEY_FIELD, SampleSpecs.HAND)
                .SetObject(HAND_DEFINITION_FIELD, content.HandGrid)
                .SetObject(HAND_SURFACE_FIELD, hand)
                .SetString(WORLD_KEY_FIELD, SampleSpecs.WORLD)
                .SetObject(WORLD_DEFINITION_FIELD, content.WorldGrid)
                .SetObject(WORLD_SURFACE_FIELD, world)
                .SetObject(MAIN_WINDOW_FIELD, mainWindow)
                .SetObject(TOGGLE_FIELD, content.ToggleInventory)
                .SetBool(WINDOW_OPENED_FIELD, false)
                .SetInt(SPAWN_COUNT_FIELD, SampleSpecs.HAND_SPAWN_COUNT);
            SerializedProperty spawnPool = serialized.FindProperty(SPAWN_POOL_FIELD);
            spawnPool.arraySize = SampleSpecs.HandSpawnPool.Length;
            for (int i = 0; i < SampleSpecs.HandSpawnPool.Length; i++)
            {
                spawnPool.GetArrayElementAtIndex(i).objectReferenceValue = content.Items[SampleSpecs.HandSpawnPool[i]];
            }

            SerializedProperty startItems = serialized.FindProperty(START_ITEMS_FIELD);
            startItems.arraySize = SampleSpecs.WorldStartItems.Length;
            for (int i = 0; i < SampleSpecs.WorldStartItems.Length; i++)
            {
                SerializedProperty item = startItems.GetArrayElementAtIndex(i);
                item.FindPropertyRelative(START_DEFINITION_FIELD).objectReferenceValue = content.Items[SampleSpecs.WorldStartItems[i].Name];
                item.FindPropertyRelative(START_ORIGIN_FIELD).vector2IntValue = SampleSpecs.WorldStartItems[i].Origin;
                item.FindPropertyRelative(START_ROTATION_FIELD).intValue = START_ROTATION;
            }

            serialized.Apply();
            return context;
        }

        private static Transform CreateChild(string name, Transform parent)
        {
            Transform child = new GameObject(name).transform;
            child.SetParent(parent, false);
            return child;
        }

        private static RectTransform CreateUI(string name, Transform parent, int layer)
        {
            GameObject target = new GameObject(name, typeof(RectTransform));
            target.layer = layer;
            RectTransform rect = (RectTransform)target.transform;
            rect.SetParent(parent, false);
            return rect;
        }

        private static RectTransform CreateStretchedUI(string name, Transform parent, int layer)
        {
            RectTransform rect = CreateUI(name, parent, layer);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }
    }
}
