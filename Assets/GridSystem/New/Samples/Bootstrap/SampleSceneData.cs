namespace GridSystem.Samples.Bootstrap
{
    using System;
    using Runtime.Definitions;
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
        private SampleInventory _hand;

        [SerializeField]
        private SampleInventory _world;

        [Header("Main window")]
        [SerializeField]
        private GameObject _mainWindow;

        [SerializeField]
        private InputActionReference _toggleMainWindow;

        [SerializeField]
        private bool _isMainWindowOpened;

        [Header("Content")]
        [SerializeField]
        private SampleStartItem[] _worldStartItems;

        [SerializeField]
        private ItemDefinition[] _handSpawnPool;

        [SerializeField, Min(0)]
        private int _handSpawnCount = 3;

        /// <summary>
        /// Toggleable main inventory.
        /// </summary>
        public SampleInventory Main => _main;

        /// <summary>
        /// Always visible hand inventory with spawned items.
        /// </summary>
        public SampleInventory Hand => _hand;

        /// <summary>
        /// World grid inventory.
        /// </summary>
        public SampleInventory World => _world;

        /// <summary>
        /// Root of the main inventory window.
        /// </summary>
        public GameObject MainWindow => _mainWindow;

        /// <summary>
        /// Action toggling the main window.
        /// </summary>
        public InputAction ToggleMainWindow => _toggleMainWindow.action;

        /// <summary>
        /// Main window state on start.
        /// </summary>
        public bool IsMainWindowOpened => _isMainWindowOpened;

        /// <summary>
        /// Items placed into the world inventory on the first launch.
        /// </summary>
        public SampleStartItem[] WorldStartItems => _worldStartItems;

        /// <summary>
        /// Definitions spawned into the hand on the first launch, without repeats.
        /// </summary>
        public ItemDefinition[] HandSpawnPool => _handSpawnPool;

        /// <summary>
        /// Amount of spawned hand items.
        /// </summary>
        public int HandSpawnCount => _handSpawnCount;
    }
}
