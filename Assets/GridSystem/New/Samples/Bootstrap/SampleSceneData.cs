namespace GridSystem.Samples.Bootstrap
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Scene references and settings of the sample.
    /// </summary>
    [Serializable]
    public sealed class SampleSceneData
    {
        [Header("Inventories")]
        [SerializeField]
        private SampleInventory _main;

        [SerializeField]
        private SampleInventory _chest;

        [SerializeField]
        private SampleInventory _hand;

        [SerializeField]
        private SampleInventory _world;

        [Header("UI Windows")]
        [SerializeField]
        private GameObject _mainWindow;

        [SerializeField]
        private GameObject _chestWindow;

        [Header("Toggle Actions")]
        [SerializeField]
        private InputActionReference _toggleMain;

        [SerializeField]
        private InputActionReference _toggleChest;

        [SerializeField]
        private InputActionReference _toggleWorld;

        [Header("Content")]
        [SerializeField]
        private SampleStartItem[] _mainStartItems;

        [SerializeField]
        private SampleStartItem[] _worldStartItems;

        [SerializeField]
        private SampleStartItem[] _handStartItems;

        /// <summary>
        /// Toggleable main inventory (I).
        /// </summary>
        public SampleInventory Main => _main;

        /// <summary>
        /// Toggleable chest inventory (E).
        /// </summary>
        public SampleInventory Chest => _chest;

        /// <summary>
        /// Always visible hand inventory with spawned items.
        /// </summary>
        public SampleInventory Hand => _hand;

        /// <summary>
        /// Toggleable world grid inventory (C).
        /// </summary>
        public SampleInventory World => _world;

        /// <summary>
        /// Root of the main inventory UI window.
        /// </summary>
        public GameObject MainWindow => _mainWindow;

        /// <summary>
        /// Root of the chest inventory UI window.
        /// </summary>
        public GameObject ChestWindow => _chestWindow;

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

        /// <summary>
        /// Items placed into the main inventory on the first launch.
        /// </summary>
        public SampleStartItem[] MainStartItems => _mainStartItems;

        /// <summary>
        /// Items placed into the world inventory on the first launch.
        /// </summary>
        public SampleStartItem[] WorldStartItems => _worldStartItems;

        /// <summary>
        /// Items placed into the hand inventory on the first launch.
        /// </summary>
        public SampleStartItem[] HandStartItems => _handStartItems;
    }
}
