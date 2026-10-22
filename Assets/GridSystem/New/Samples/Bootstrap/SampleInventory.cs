namespace GridSystem.Samples.Bootstrap
{
    using System;
    using Runtime.Definitions;
    using UnityEngine;

    /// <summary>
    /// Inventory created by the sample. Holds no scene reference, so it lives in a config asset.
    /// The surface showing it comes from the spawned content.
    /// </summary>
    [Serializable]
    public sealed class SampleInventory
    {
        [SerializeField]
        private string _key;

        [SerializeField]
        private GridDefinition _definition;

        [SerializeField]
        private SampleStartItem[] _startItems = Array.Empty<SampleStartItem>();

        /// <summary>
        /// Persistent save key.
        /// </summary>
        public string Key => _key;

        /// <summary>
        /// Grid size.
        /// </summary>
        public GridDefinition Definition => _definition;

        /// <summary>
        /// Items placed into the inventory on the first launch.
        /// </summary>
        public SampleStartItem[] StartItems => _startItems;
    }
}
