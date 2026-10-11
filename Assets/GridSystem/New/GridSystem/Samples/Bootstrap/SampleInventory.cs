namespace GridSystem.Samples.Bootstrap
{
    using System;
    using Runtime.Definitions;
    using Runtime.Surfaces;
    using UnityEngine;

    /// <summary>
    /// Inventory created by the sample and the surface showing it.
    /// </summary>
    [Serializable]
    public sealed class SampleInventory
    {
        [SerializeField]
        private string _key;

        [SerializeField]
        private GridDefinition _definition;

        [SerializeField]
        private GridSurface _surface;

        /// <summary>
        /// Persistent save key.
        /// </summary>
        public string Key => _key;

        /// <summary>
        /// Grid size.
        /// </summary>
        public GridDefinition Definition => _definition;

        /// <summary>
        /// Surface showing the inventory.
        /// </summary>
        public GridSurface Surface => _surface;
    }
}
