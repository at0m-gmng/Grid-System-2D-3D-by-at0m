namespace GridSystem.Samples.Editor
{
    using Description;
    using Runtime.Composition;
    using Runtime.Definitions;
    using Runtime.Surfaces;
    using TMPro;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;

    /// <summary>
    /// Creates DI independent scene objects mirroring the old example hierarchy:
    /// Environment (camera, light, WorldPrefab), WindowsSystem (MainInventory, OthersInventories, OutsideParent, InventoryItemView),
    /// PoolParent and GridSystem context.
    /// </summary>
    public sealed class SampleSceneBuilder
    {
        private const string ENVIRONMENT_NAME = "Environment";
        private const string CAMERA_NAME = "Main Camera";
        private const string LIGHT_NAME = "Directional Light";
        private const string WORLD_NAME = "WorldPrefab";
        private const string WINDOWS_NAME = "WindowsSystem";
        private const string MAIN_NAME = "MainInventory";
        private const string CHEST_NAME = "OthersInventories";
        private const string HAND_NAME = "OutsideParent";
        private const string DESCRIPTION_NAME = "InventoryItemView";
        private const string POPUP_NAME = "Popup";
        private const string ROW_NAME = "DataContainer";
        private const string LABEL_NAME = "Label";
        private const string VALUE_NAME = "Value";
        private const string GRID_NAME = "Grid";
        private const string CELLS_NAME = "Cells";
        private const string ITEMS_NAME = "Items";
        private const string POOL_NAME = "PoolParent";
        private const string GRID_SYSTEM_NAME = "GridSystem";
        private const string NAME_LABEL = "Name:";
        private const string DAMAGE_LABEL = "Damage:";
        private const string ATTACK_SPEED_LABEL = "Attack Speed:";
        private const string RANGE_LABEL = "Range:";
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
        private const string NAME_FIELD = "_name";
        private const string DAMAGE_FIELD = "_damage";
        private const string ATTACK_SPEED_FIELD = "_attackSpeed";
        private const string RANGE_FIELD = "_range";
        private const string SETTINGS_FIELD = "_settings";
        private const string POOL_ROOT_FIELD = "_poolRoot";
        private const string MODULES_FIELD = "_modules";
        private const string DESCRIPTION_FIELD = "_description";
        private const string HAND_FIELD = "_data._hand";
        private const string SPAWN_POOL_FIELD = "_data._handSpawnPool";
        private const string SPAWN_COUNT_FIELD = "_data._handSpawnCount";
        private const string WINDOWS_FIELD = "_data._windows";
        private const string INVENTORY_FIELD = "_inventory";
        private const string KEY_FIELD = "_key";
        private const string DEFINITION_FIELD = "_definition";
        private const string SURFACE_FIELD = "_surface";
        private const string WINDOW_ROOT_FIELD = "_root";
        private const string TOGGLE_FIELD = "_toggle";
        private const string OPENED_FIELD = "_isOpenedOnStart";
        private const string START_ITEMS_FIELD = "_startItems";
        private const string START_DEFINITION_FIELD = "Definition";
        private const string START_ORIGIN_FIELD = "Origin";
        private const string START_ROTATION_FIELD = "Rotation";
        private const string FIELD_SEPARATOR = ".";
        private const int START_ROTATION = 0;
        private const int MAIN_WINDOW = 0;
        private const int CHEST_WINDOW = 1;
        private const int WORLD_WINDOW = 2;
        private const int WINDOWS_COUNT = 3;
        private const bool IS_OPENED_ON_START = false;

        /// <summary>
        /// Builds scene objects in the active scene and returns the context to install into a container.
        /// </summary>
        public GridSystemContext Build(SampleContent content)
        {
            Transform environment = new GameObject(ENVIRONMENT_NAME).transform;
            Camera camera = CreateCamera(environment);
            CreateLight(environment);
            WorldGridSurface world = CreateWorld(environment, content, camera);
            int uiLayer = LayerMask.NameToLayer(SampleLayout.UI_LAYER);
            RectTransform windows = CreateCanvas(uiLayer);
            UIGridSurface main = CreateWindow(MAIN_NAME, windows, uiLayer, SampleLayout.Center, SampleLayout.MainPosition, SampleSpecs.MainSize, content);
            UIGridSurface chest = CreateWindow(CHEST_NAME, windows, uiLayer, SampleLayout.Center, SampleLayout.ChestPosition, SampleSpecs.ChestSize, content);
            UIGridSurface hand = CreateWindow(HAND_NAME, windows, uiLayer, SampleLayout.BottomCenter, SampleLayout.HandPosition, SampleSpecs.HandSize, content);
            ItemDescriptionView description = CreateDescription(windows, uiLayer);
            GameObject pool = new GameObject(POOL_NAME);
            pool.SetActive(false);
            GameObject target = new GameObject(GRID_SYSTEM_NAME);
            GridSystemContext context = target.AddComponent<GridSystemContext>();
            SampleModule module = target.AddComponent<SampleModule>();
            new SerializedObject(context)
                .SetObject(SETTINGS_FIELD, content.Settings)
                .SetObject(POOL_ROOT_FIELD, pool.transform)
                .SetObjects(MODULES_FIELD, new GridModule[] { module })
                .Apply();
            SerializedObject serialized = new SerializedObject(module).SetObject(DESCRIPTION_FIELD, description);
            SetInventory(serialized.FindProperty(HAND_FIELD), SampleSpecs.HAND, content.HandGrid, hand);
            SetItems(serialized.FindProperty(SPAWN_POOL_FIELD), SampleSpecs.HandSpawnPool, content);
            serialized.SetInt(SPAWN_COUNT_FIELD, SampleSpecs.HAND_SPAWN_COUNT);
            SerializedProperty windowsProperty = serialized.FindProperty(WINDOWS_FIELD);
            windowsProperty.arraySize = WINDOWS_COUNT;
            SetWindow(windowsProperty.GetArrayElementAtIndex(MAIN_WINDOW), SampleSpecs.MAIN, content.MainGrid, main, main.transform.parent.gameObject,
                content.ToggleMain, NoItems(), content);
            SetWindow(windowsProperty.GetArrayElementAtIndex(CHEST_WINDOW), SampleSpecs.CHEST, content.ChestGrid, chest,
                chest.transform.parent.gameObject, content.ToggleChest, SampleSpecs.ChestStartItems, content);
            SetWindow(windowsProperty.GetArrayElementAtIndex(WORLD_WINDOW), SampleSpecs.WORLD, content.WorldGrid, world, world.gameObject,
                content.ToggleWorld, SampleSpecs.WorldStartItems, content);
            serialized.Apply();
            return context;
        }

        private static Camera CreateCamera(Transform parent)
        {
            GameObject target = new GameObject(CAMERA_NAME);
            target.tag = SampleLayout.MAIN_CAMERA_TAG;
            target.transform.SetParent(parent, false);
            target.transform.SetPositionAndRotation(SampleLayout.CameraPosition, Quaternion.Euler(SampleLayout.CameraRotation));
            target.AddComponent<AudioListener>();
            return target.AddComponent<Camera>();
        }

        private static void CreateLight(Transform parent)
        {
            GameObject target = new GameObject(LIGHT_NAME);
            target.transform.SetParent(parent, false);
            target.transform.rotation = Quaternion.Euler(SampleLayout.LightRotation);
            target.AddComponent<Light>().type = LightType.Directional;
        }

        private static WorldGridSurface CreateWorld(Transform parent, SampleContent content, Camera camera)
        {
            Transform root = CreateChild(WORLD_NAME, parent);
            Transform cells = CreateChild(CELLS_NAME, root);
            Transform items = CreateChild(ITEMS_NAME, root);
            WorldGridSurface surface = root.gameObject.AddComponent<WorldGridSurface>();
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
            GameObject target = new GameObject(WINDOWS_NAME, typeof(RectTransform));
            target.layer = layer;
            target.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = target.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = SampleLayout.ReferenceResolution;
            scaler.matchWidthOrHeight = SampleLayout.SCALER_MATCH;
            return (RectTransform)target.transform;
        }

        private static UIGridSurface CreateWindow(string name, Transform parent, int layer, Vector2 anchor, Vector2 position, Vector2Int gridSize,
            SampleContent content)
        {
            RectTransform window = CreateUI(name, parent, layer);
            window.anchorMin = anchor;
            window.anchorMax = anchor;
            window.anchoredPosition = position;
            window.sizeDelta = GetGridSize(gridSize) + SampleLayout.WindowPadding;
            Image background = window.gameObject.AddComponent<Image>();
            background.color = SampleLayout.WindowColor;
            background.raycastTarget = false;

            RectTransform grid = CreateUI(GRID_NAME, window, layer);
            grid.anchorMin = SampleLayout.Center;
            grid.anchorMax = SampleLayout.Center;
            grid.pivot = SampleLayout.Center;
            RectTransform cells = CreateStretchedUI(CELLS_NAME, grid, layer);
            RectTransform items = CreateStretchedUI(ITEMS_NAME, grid, layer);
            UIGridSurface surface = grid.gameObject.AddComponent<UIGridSurface>();
            new SerializedObject(surface)
                .SetObject(VIEWS_FIELD, content.UIViews)
                .SetObject(ITEMS_ROOT_FIELD, items)
                .SetObject(CELLS_ROOT_FIELD, cells)
                .SetObject(CELL_PREFAB_FIELD, content.UICell)
                .SetVector2(CELL_SIZE_FIELD, SampleLayout.UICellSize)
                .SetVector2(SPACING_FIELD, SampleLayout.UISpacing)
                .SetInt(PRIORITY_FIELD, SampleLayout.UI_PRIORITY)
                .SetObject(RECT_FIELD, grid)
                .Apply();
            return surface;
        }

        private static ItemDescriptionView CreateDescription(Transform parent, int layer)
        {
            RectTransform root = CreateStretchedUI(DESCRIPTION_NAME, parent, layer);
            RectTransform popup = CreateUI(POPUP_NAME, root, layer);
            popup.anchorMin = SampleLayout.TopCenter;
            popup.anchorMax = SampleLayout.TopCenter;
            popup.pivot = SampleLayout.TopCenter;
            popup.anchoredPosition = SampleLayout.DescriptionPosition;
            popup.sizeDelta = SampleLayout.DescriptionSize;
            Image background = popup.gameObject.AddComponent<Image>();
            background.color = SampleLayout.DescriptionColor;
            background.raycastTarget = false;
            VerticalLayoutGroup layout = popup.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(SampleLayout.DESCRIPTION_PADDING, SampleLayout.DESCRIPTION_PADDING, SampleLayout.DESCRIPTION_PADDING,
                SampleLayout.DESCRIPTION_PADDING);
            ItemDescriptionView view = root.gameObject.AddComponent<ItemDescriptionView>();
            new SerializedObject(view)
                .SetObject(ROOT_FIELD, popup.gameObject)
                .SetObject(NAME_FIELD, CreateRow(popup, layer, NAME_LABEL))
                .SetObject(DAMAGE_FIELD, CreateRow(popup, layer, DAMAGE_LABEL))
                .SetObject(ATTACK_SPEED_FIELD, CreateRow(popup, layer, ATTACK_SPEED_LABEL))
                .SetObject(RANGE_FIELD, CreateRow(popup, layer, RANGE_LABEL))
                .Apply();
            return view;
        }

        private static TMP_Text CreateRow(Transform parent, int layer, string label)
        {
            RectTransform row = CreateUI(ROW_NAME, parent, layer);
            RectTransform labelRect = CreateStretchedUI(LABEL_NAME, row, layer);
            labelRect.anchorMax = SampleLayout.LeftHalfMax;
            CreateText(labelRect).text = label;
            RectTransform valueRect = CreateStretchedUI(VALUE_NAME, row, layer);
            valueRect.anchorMin = SampleLayout.RightHalfMin;
            return CreateText(valueRect);
        }

        private static TMP_Text CreateText(RectTransform target)
        {
            TextMeshProUGUI text = target.gameObject.AddComponent<TextMeshProUGUI>();
            text.fontSize = SampleLayout.DESCRIPTION_FONT_SIZE;
            text.color = SampleLayout.DescriptionTextColor;
            text.raycastTarget = false;
            return text;
        }

        private static void SetWindow(SerializedProperty window, string key, GridDefinition definition, GridSurface surface, GameObject root,
            InputActionReference toggle, (string Name, Vector2Int Origin)[] startItems, SampleContent content)
        {
            SetInventory(window.FindPropertyRelative(INVENTORY_FIELD), key, definition, surface);
            window.FindPropertyRelative(WINDOW_ROOT_FIELD).objectReferenceValue = root;
            window.FindPropertyRelative(TOGGLE_FIELD).objectReferenceValue = toggle;
            window.FindPropertyRelative(OPENED_FIELD).boolValue = IS_OPENED_ON_START;
            SerializedProperty items = window.FindPropertyRelative(START_ITEMS_FIELD);
            items.arraySize = startItems.Length;
            for (int i = 0; i < startItems.Length; i++)
            {
                SerializedProperty item = items.GetArrayElementAtIndex(i);
                item.FindPropertyRelative(START_DEFINITION_FIELD).objectReferenceValue = content.Items[startItems[i].Name];
                item.FindPropertyRelative(START_ORIGIN_FIELD).vector2IntValue = startItems[i].Origin;
                item.FindPropertyRelative(START_ROTATION_FIELD).intValue = START_ROTATION;
            }
        }

        private static void SetInventory(SerializedProperty inventory, string key, GridDefinition definition, GridSurface surface)
        {
            inventory.FindPropertyRelative(KEY_FIELD).stringValue = key;
            inventory.FindPropertyRelative(DEFINITION_FIELD).objectReferenceValue = definition;
            inventory.FindPropertyRelative(SURFACE_FIELD).objectReferenceValue = surface;
        }

        private static void SetItems(SerializedProperty array, string[] names, SampleContent content)
        {
            array.arraySize = names.Length;
            for (int i = 0; i < names.Length; i++)
            {
                array.GetArrayElementAtIndex(i).objectReferenceValue = content.Items[names[i]];
            }
        }

        private static (string Name, Vector2Int Origin)[] NoItems() => new (string Name, Vector2Int Origin)[0];

        private static Vector2 GetGridSize(Vector2Int cells)
            => new Vector2(cells.x * (SampleLayout.UICellSize.x + SampleLayout.UISpacing.x) - SampleLayout.UISpacing.x,
                cells.y * (SampleLayout.UICellSize.y + SampleLayout.UISpacing.y) - SampleLayout.UISpacing.y);

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
