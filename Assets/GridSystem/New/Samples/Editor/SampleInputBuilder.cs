namespace GridSystem.Samples.Editor
{
    using System.IO;
    using Runtime.Pointer;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Creates the input actions asset and the pointer input config.
    /// </summary>
    public sealed class SampleInputBuilder
    {
        private const string ASSET_NAME = "GridSystemInput";
        private const string ASSET_EXTENSION = ".inputactions";
        private const string CONFIG_NAME = "PointerInputConfig";
        private const string MAP = "Grid";
        private const string POINT = "Point";
        private const string PRESS = "Press";
        private const string ROTATE = "Rotate";
        private const string TOGGLE = "ToggleInventory";
        private const string VECTOR2_LAYOUT = "Vector2";
        private const string POINTER_POSITION = "<Pointer>/position";
        private const string MOUSE_LEFT = "<Mouse>/leftButton";
        private const string TOUCH_PRESS = "<Touchscreen>/primaryTouch/press";
        private const string KEY_R = "<Keyboard>/r";
        private const string MOUSE_RIGHT = "<Mouse>/rightButton";
        private const string KEY_I = "<Keyboard>/i";
        private const string POINT_FIELD = "_point";
        private const string PRESS_FIELD = "_press";
        private const string ROTATE_FIELD = "_rotate";

        /// <summary>
        /// Creates missing input assets and fills the content.
        /// </summary>
        public PointerInputConfig Build(SampleContent content)
        {
            string path = SamplePaths.Combine(SamplePaths.INPUT, ASSET_NAME, ASSET_EXTENSION);
            if (AssetDatabase.LoadAssetAtPath<InputActionAsset>(path) == null)
            {
                CreateActionsAsset(path);
            }

            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            PointerInputConfig config = SampleAssets.LoadOrCreate<PointerInputConfig>(SamplePaths.INPUT, CONFIG_NAME);
            new SerializedObject(config)
                .SetObject(POINT_FIELD, FindReference(assets, POINT))
                .SetObject(PRESS_FIELD, FindReference(assets, PRESS))
                .SetObject(ROTATE_FIELD, FindReference(assets, ROTATE))
                .Apply();
            content.ToggleInventory = FindReference(assets, TOGGLE);
            return config;
        }

        private static void CreateActionsAsset(string path)
        {
            InputActionAsset asset = ScriptableObject.CreateInstance<InputActionAsset>();
            InputActionMap map = asset.AddActionMap(MAP);
            map.AddAction(POINT, InputActionType.Value, POINTER_POSITION, expectedControlLayout: VECTOR2_LAYOUT);
            InputAction press = map.AddAction(PRESS, InputActionType.Button, MOUSE_LEFT);
            press.AddBinding(TOUCH_PRESS);
            InputAction rotate = map.AddAction(ROTATE, InputActionType.Button, KEY_R);
            rotate.AddBinding(MOUSE_RIGHT);
            map.AddAction(TOGGLE, InputActionType.Button, KEY_I);
            SampleAssets.EnsureFolder(SamplePaths.INPUT);
            File.WriteAllText(path, asset.ToJson());
            Object.DestroyImmediate(asset);
            AssetDatabase.ImportAsset(path);
        }

        private static InputActionReference FindReference(Object[] assets, string actionName)
        {
            InputActionReference result = null;
            for (int i = 0; result == null && i < assets.Length; i++)
            {
                if (assets[i] is InputActionReference reference && reference.action.name == actionName)
                {
                    result = reference;
                }
            }

            return result;
        }
    }
}
