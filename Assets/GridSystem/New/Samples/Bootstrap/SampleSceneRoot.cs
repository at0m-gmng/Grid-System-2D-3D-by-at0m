namespace GridSystem.Samples.Bootstrap
{
    using Runtime.Surfaces;
    using Tooltip;
    using UnityEngine;

    /// <summary>
    /// Root of the spawned sample UI. Exposes the parts that only the prefab can describe,
    /// so nothing outside has to hold a scene reference into the hierarchy.
    /// </summary>
    public sealed class SampleSceneRoot : MonoBehaviour
    {
        [Header("Surfaces")]
        [SerializeField]
        private GridSurface _main;

        [SerializeField]
        private GridSurface _chest;

        [SerializeField]
        private GridSurface _hand;

        [Header("Windows")]
        [SerializeField]
        private GameObject _mainWindow;

        [SerializeField]
        private GameObject _chestWindow;

        [Header("Tooltip")]
        [SerializeField]
        private ItemTooltipView _tooltip;

        /// <summary>
        /// Surface of the toggleable main inventory.
        /// </summary>
        public GridSurface Main => _main;

        /// <summary>
        /// Surface of the toggleable chest inventory.
        /// </summary>
        public GridSurface Chest => _chest;

        /// <summary>
        /// Surface of the always visible hand bar.
        /// </summary>
        public GridSurface Hand => _hand;

        /// <summary>
        /// Root of the main inventory window.
        /// </summary>
        public GameObject MainWindow => _mainWindow;

        /// <summary>
        /// Root of the chest inventory window.
        /// </summary>
        public GameObject ChestWindow => _chestWindow;

        /// <summary>
        /// Tooltip widget of the sample.
        /// </summary>
        public ItemTooltipView Tooltip => _tooltip;
    }
}
