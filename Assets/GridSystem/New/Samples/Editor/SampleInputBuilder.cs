namespace GridSystem.Samples.Editor
{
    using Runtime.Pointer;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Creates the pointer input config from actions of the existing GameInput asset.
    /// </summary>
    public sealed class SampleInputBuilder
    {
        private const string CONFIG_NAME = "PointerInputConfig";
        private const string POINT_ACTION = "Moved";
        private const string PRESS_ACTION = "LMB_Hold";
        private const string ROTATE_ACTION = "Rotation";
        private const string INVENTORY_ACTION = "Inventory";
        private const string INTERACTION_ACTION = "Interaction";
        private const string WORLD_ACTION = "SLWorld";
        private const string POINT_FIELD = "_point";
        private const string PRESS_FIELD = "_press";
        private const string ROTATE_FIELD = "_rotate";

        /// <summary>
        /// Creates or updates the pointer input config and fills window toggles of the content.
        /// </summary>
        public PointerInputConfig Build(SampleContent content)
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(SamplePaths.INPUT_ACTIONS);
            PointerInputConfig config = SampleAssets.LoadOrCreate<PointerInputConfig>(SamplePaths.INPUT, CONFIG_NAME);
            new SerializedObject(config)
                .SetObject(POINT_FIELD, FindReference(assets, POINT_ACTION))
                .SetObject(PRESS_FIELD, FindReference(assets, PRESS_ACTION))
                .SetObject(ROTATE_FIELD, FindReference(assets, ROTATE_ACTION))
                .Apply();
            content.ToggleMain = FindReference(assets, INVENTORY_ACTION);
            content.ToggleChest = FindReference(assets, INTERACTION_ACTION);
            content.ToggleWorld = FindReference(assets, WORLD_ACTION);
            return config;
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
