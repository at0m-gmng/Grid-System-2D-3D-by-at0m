namespace GridSystem.Samples.Editor
{
    /// <summary>
    /// Asset locations used by the sample generator.
    /// </summary>
    public static class SamplePaths
    {
        /// <summary>
        /// Root folder of generated content.
        /// </summary>
        public const string CONTENT = "Assets/GridSystem/New/Samples/Content";

        /// <summary>
        /// Pointer input config folder.
        /// </summary>
        public const string INPUT = CONTENT + "/Input";

        /// <summary>
        /// Item definitions, catalog and merge rules folder.
        /// </summary>
        public const string DEFINITIONS = CONTENT + "/Definitions";

        /// <summary>
        /// Grid definitions folder.
        /// </summary>
        public const string GRIDS = CONTENT + "/Grids";

        /// <summary>
        /// UI prefabs folder.
        /// </summary>
        public const string UI_PREFABS = CONTENT + "/Prefabs/UI";

        /// <summary>
        /// World prefabs folder.
        /// </summary>
        public const string WORLD_PREFABS = CONTENT + "/Prefabs/World";

        /// <summary>
        /// View catalogs folder.
        /// </summary>
        public const string CATALOGS = CONTENT + "/Catalogs";

        /// <summary>
        /// Scenes folder.
        /// </summary>
        public const string SCENES = CONTENT + "/Scenes";

        /// <summary>
        /// Input actions of the old example: DragDrop (LMB_Hold, Moved, Rotation) and Control (Inventory, Interaction, SLWorld) maps.
        /// </summary>
        public const string INPUT_ACTIONS = "Assets/GridSystem/New/Samples/Input/GameInput.inputactions";

        /// <summary>
        /// Item sprites of the old example.
        /// </summary>
        public const string ITEM_SPRITES = "Assets/GridSystem/Examples/Textures/Items";

        /// <summary>
        /// Cell sprite of the old example.
        /// </summary>
        public const string CELL_SPRITE = "Assets/GridSystem/Examples/Textures/Grid/GridBackground.psd";

        /// <summary>
        /// Cell material of the old example.
        /// </summary>
        public const string CELL_MATERIAL = "Assets/GridSystem/Examples/Materials/GridCellMaterial.mat";

        /// <summary>
        /// Item material of the old example.
        /// </summary>
        public const string ITEM_MATERIAL = "Assets/GridSystem/Examples/Materials/GridItemMaterial.mat";

        /// <summary>
        /// Asset file extension.
        /// </summary>
        public const string ASSET_EXTENSION = ".asset";

        /// <summary>
        /// Prefab file extension.
        /// </summary>
        public const string PREFAB_EXTENSION = ".prefab";

        /// <summary>
        /// Scene file extension.
        /// </summary>
        public const string SCENE_EXTENSION = ".unity";

        /// <summary>
        /// Path separator of asset paths.
        /// </summary>
        public const string SEPARATOR = "/";

        /// <summary>
        /// Builds an asset path inside the folder.
        /// </summary>
        public static string Combine(string folder, string name, string extension) => folder + SEPARATOR + name + extension;
    }
}
