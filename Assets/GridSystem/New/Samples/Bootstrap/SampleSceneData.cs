namespace GridSystem.Samples.Bootstrap
{
    using System;
    using Runtime.Definitions;
    using UnityEngine;

    /// <summary>
    /// Scene references and settings of the sample.
    /// </summary>
    [Serializable]
    public sealed class SampleSceneData
    {
        [Header("Always opened inventory with spawned items")]
        [SerializeField]
        private SampleInventory _hand;

        [SerializeField]
        private ItemDefinition[] _handSpawnPool = Array.Empty<ItemDefinition>();

        [SerializeField, Min(0)]
        private int _handSpawnCount;

        [Header("Inventories toggled by input")]
        [SerializeField]
        private SampleWindow[] _windows = Array.Empty<SampleWindow>();

        /// <summary>
        /// Always opened inventory.
        /// </summary>
        public SampleInventory Hand => _hand;

        /// <summary>
        /// Definitions spawned into the hand on the first launch, without repeats.
        /// </summary>
        public ItemDefinition[] HandSpawnPool => _handSpawnPool;

        /// <summary>
        /// Amount of spawned hand items.
        /// </summary>
        public int HandSpawnCount => _handSpawnCount;

        /// <summary>
        /// Inventories toggled by input.
        /// </summary>
        public SampleWindow[] Windows => _windows;
    }
}
