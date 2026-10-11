namespace GridSystem.Runtime.Persistence
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Serializable inventory state.
    /// </summary>
    [Serializable]
    public sealed class InventorySnapshot
    {
        /// <summary>
        /// Placed items.
        /// </summary>
        public List<ItemSnapshot> Items = new();
    }

    /// <summary>
    /// Serializable item state.
    /// </summary>
    [Serializable]
    public struct ItemSnapshot
    {
        public ItemSnapshot(string definitionId, Vector2Int origin, int rotation)
        {
            DefinitionId = definitionId;
            Origin = origin;
            Rotation = rotation;
        }

        /// <summary>
        /// Persistent definition id.
        /// </summary>
        public string DefinitionId;

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
