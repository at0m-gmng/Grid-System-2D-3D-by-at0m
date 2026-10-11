namespace GridSystem.Samples.Editor
{
    using UnityEngine;

    /// <summary>
    /// Transform, size and color values of generated prefabs and scene objects.
    /// The scene mirrors the old example: Environment, WindowsSystem, MainInventory, OthersInventories, OutsideParent, InventoryItemView.
    /// </summary>
    public static class SampleLayout
    {
        /// <summary>
        /// Anchor and pivot at the center.
        /// </summary>
        public static readonly Vector2 Center = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// Anchor at the bottom center.
        /// </summary>
        public static readonly Vector2 BottomCenter = new Vector2(0.5f, 0f);

        /// <summary>
        /// Anchor and pivot at the top center.
        /// </summary>
        public static readonly Vector2 TopCenter = new Vector2(0.5f, 1f);

        /// <summary>
        /// Left half anchor maximum.
        /// </summary>
        public static readonly Vector2 LeftHalfMax = new Vector2(0.5f, 1f);

        /// <summary>
        /// Right half anchor minimum.
        /// </summary>
        public static readonly Vector2 RightHalfMin = new Vector2(0.5f, 0f);

        /// <summary>
        /// UI cell size.
        /// </summary>
        public static readonly Vector2 UICellSize = new Vector2(80f, 80f);

        /// <summary>
        /// UI cell spacing.
        /// </summary>
        public static readonly Vector2 UISpacing = new Vector2(4f, 4f);

        /// <summary>
        /// Window padding around the grid.
        /// </summary>
        public static readonly Vector2 WindowPadding = new Vector2(40f, 40f);

        /// <summary>
        /// World cell size.
        /// </summary>
        public static readonly Vector2 WorldCellSize = Vector2.one;

        /// <summary>
        /// World cell spacing.
        /// </summary>
        public static readonly Vector2 WorldSpacing = new Vector2(0.05f, 0.05f);

        /// <summary>
        /// Picking priority of UI surfaces, above the world.
        /// </summary>
        public const int UI_PRIORITY = 10;

        /// <summary>
        /// Picking priority of the world surface.
        /// </summary>
        public const int WORLD_PRIORITY = 0;

        /// <summary>
        /// Quad rotation lying on XZ plane.
        /// </summary>
        public static readonly Vector3 QuadRotation = new Vector3(90f, 0f, 0f);

        /// <summary>
        /// Quad lift above the surface to avoid z-fighting.
        /// </summary>
        public static readonly Vector3 QuadOffset = new Vector3(0f, 0.001f, 0f);

        /// <summary>
        /// Cube footprint inside a cell.
        /// </summary>
        public const float MODEL_FOOTPRINT = 0.9f;

        /// <summary>
        /// Half multiplier.
        /// </summary>
        public const float HALF = 0.5f;

        /// <summary>
        /// Camera position.
        /// </summary>
        public static readonly Vector3 CameraPosition = new Vector3(0f, 12f, -9f);

        /// <summary>
        /// Camera rotation.
        /// </summary>
        public static readonly Vector3 CameraRotation = new Vector3(55f, 0f, 0f);

        /// <summary>
        /// Light rotation.
        /// </summary>
        public static readonly Vector3 LightRotation = new Vector3(50f, -30f, 0f);

        /// <summary>
        /// Canvas reference resolution.
        /// </summary>
        public static readonly Vector2 ReferenceResolution = new Vector2(1920f, 1080f);

        /// <summary>
        /// Canvas width and height match.
        /// </summary>
        public const float SCALER_MATCH = 0.5f;

        /// <summary>
        /// Main inventory window position.
        /// </summary>
        public static readonly Vector2 MainPosition = new Vector2(-330f, 40f);

        /// <summary>
        /// Chest inventory window position.
        /// </summary>
        public static readonly Vector2 ChestPosition = new Vector2(330f, 40f);

        /// <summary>
        /// Hand panel position from the bottom edge.
        /// </summary>
        public static readonly Vector2 HandPosition = new Vector2(0f, 150f);

        /// <summary>
        /// Description popup position from the top edge.
        /// </summary>
        public static readonly Vector2 DescriptionPosition = new Vector2(0f, -20f);

        /// <summary>
        /// Description popup size.
        /// </summary>
        public static readonly Vector2 DescriptionSize = new Vector2(500f, 250f);

        /// <summary>
        /// Description popup padding.
        /// </summary>
        public const int DESCRIPTION_PADDING = 20;

        /// <summary>
        /// Description font size.
        /// </summary>
        public const float DESCRIPTION_FONT_SIZE = 24f;

        /// <summary>
        /// Window background color.
        /// </summary>
        public static readonly Color WindowColor = new Color(0.1f, 0.1f, 0.12f, 0.85f);

        /// <summary>
        /// Description background color of the old example.
        /// </summary>
        public static readonly Color DescriptionColor = new Color(0.4641509f, 0.4641509f, 0.4641509f, 1f);

        /// <summary>
        /// Description text color of the old example.
        /// </summary>
        public static readonly Color DescriptionTextColor = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);

        /// <summary>
        /// Name of the UI layer.
        /// </summary>
        public const string UI_LAYER = "UI";

        /// <summary>
        /// Tag of the main camera.
        /// </summary>
        public const string MAIN_CAMERA_TAG = "MainCamera";

        /// <summary>
        /// Color property of the Built-in render pipeline.
        /// </summary>
        public const string BUILT_IN_COLOR = "_Color";

        /// <summary>
        /// Color property of URP and HDRP.
        /// </summary>
        public const string SRP_COLOR = "_BaseColor";
    }
}
