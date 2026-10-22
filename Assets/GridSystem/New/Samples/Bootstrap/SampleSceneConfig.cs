namespace GridSystem.Samples.Bootstrap
{
    using Runtime.Surfaces;
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Everything the sample needs that is not a scene reference: content prefabs, inventories,
    /// start items and input. Lives in an asset, so every container scene shares one configuration.
    /// </summary>
    [CreateAssetMenu(menuName = MENU_NAME, fileName = FILE_NAME)]
    public sealed class SampleSceneConfig : ScriptableObject
    {
        private const string MENU_NAME = "Grid System/Sample Scene Config";
        private const string FILE_NAME = "SampleSceneConfig";

        [Header("Content")]
        [SerializeField]
        private SampleSceneRoot _uiPrefab;

        [SerializeField]
        private GridSurface _worldPrefab;

        [Header("Inventories")]
        [SerializeField]
        private SampleInventory _main;

        [SerializeField]
        private SampleInventory _chest;

        [SerializeField]
        private SampleInventory _hand;

        [SerializeField]
        private SampleInventory _world;

        [Header("Toggle Actions")]
        [SerializeField]
        private InputActionReference _toggleMain;

        [SerializeField]
        private InputActionReference _toggleChest;

        [SerializeField]
        private InputActionReference _toggleWorld;

        /// <summary>
        /// UI prefab spawned at startup.
        /// </summary>
        public SampleSceneRoot UiPrefab => _uiPrefab;

        /// <summary>
        /// World grid prefab spawned at startup.
        /// </summary>
        public GridSurface WorldPrefab => _worldPrefab;

        /// <summary>
        /// Toggleable main inventory (I).
        /// </summary>
        public SampleInventory Main => _main;

        /// <summary>
        /// Toggleable chest inventory (E).
        /// </summary>
        public SampleInventory Chest => _chest;

        /// <summary>
        /// Always visible hand bar.
        /// </summary>
        public SampleInventory Hand => _hand;

        /// <summary>
        /// Toggleable world grid inventory (C).
        /// </summary>
        public SampleInventory World => _world;

        /// <summary>
        /// Action toggling the main window.
        /// </summary>
        public InputAction ToggleMain => _toggleMain.action;

        /// <summary>
        /// Action toggling the chest window.
        /// </summary>
        public InputAction ToggleChest => _toggleChest.action;

        /// <summary>
        /// Action toggling the world inventory.
        /// </summary>
        public InputAction ToggleWorld => _toggleWorld.action;
    }
}
