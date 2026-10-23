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

        [Tooltip("Off for a bar that is refilled from start items on every launch instead of being restored.")]
        [SerializeField]
        private bool _isPersistent = true;

        /// <summary>
        /// Persistent save key.
        /// </summary>
        public string Key => _key;

        /// <summary>
        /// Keeps the content between sessions. When off the inventory is never stored and never restored,
        /// so it starts from its start items every launch.
        /// </summary>
        public bool IsPersistent => _isPersistent;

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
