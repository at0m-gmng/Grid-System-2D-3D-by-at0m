namespace GridSystem.Samples.Bootstrap
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Inventory opened and closed by an input action. The root is visible while the inventory is opened.
    /// </summary>
    [Serializable]
    public sealed class SampleWindow
    {
        [SerializeField]
        private SampleInventory _inventory;

        [SerializeField]
        private GameObject _root;

        [SerializeField]
        private InputActionReference _toggle;

        [SerializeField]
        private bool _isOpenedOnStart;

        [SerializeField]
        private SampleStartItem[] _startItems = Array.Empty<SampleStartItem>();

        /// <summary>
        /// Shown inventory.
        /// </summary>
        public SampleInventory Inventory => _inventory;

        /// <summary>
        /// Object shown while the inventory is opened.
        /// </summary>
        public GameObject Root => _root;

        /// <summary>
        /// Action toggling the inventory.
        /// </summary>
        public InputAction Toggle => _toggle.action;

        /// <summary>
        /// Opened state on start.
        /// </summary>
        public bool IsOpenedOnStart => _isOpenedOnStart;

        /// <summary>
        /// Items placed on the first launch.
        /// </summary>
        public SampleStartItem[] StartItems => _startItems;
    }
}
