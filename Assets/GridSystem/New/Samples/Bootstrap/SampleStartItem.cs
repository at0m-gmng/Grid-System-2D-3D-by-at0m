namespace GridSystem.Samples.Bootstrap
{
    using System;
    using Runtime.Definitions;
    using UnityEngine;

    /// <summary>
    /// Item placed into an empty inventory on the first launch.
    /// </summary>
    [Serializable]
    public struct SampleStartItem
    {
        /// <summary>
        /// Item definition.
        /// </summary>
        public ItemDefinition Definition;

        /// <summary>
        /// Pivot cell.
        /// </summary>
        public Vector2Int Origin;

        /// <summary>
        /// Clockwise quarter turns.
        /// </summary>
        public int Rotation;
    }
}
