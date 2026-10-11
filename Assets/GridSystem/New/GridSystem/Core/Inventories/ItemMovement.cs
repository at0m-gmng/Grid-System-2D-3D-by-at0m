namespace GridSystem.Core.Inventories
{
    using Items;
    using UnityEngine;

    /// <summary>
    /// Allocation free description of an item move inside one inventory.
    /// </summary>
    public readonly struct ItemMovement
    {
        public ItemMovement(GridItem item, Vector2Int previousOrigin, int previousRotation)
        {
            Item = item;
            PreviousOrigin = previousOrigin;
            PreviousRotation = previousRotation;
        }

        /// <summary>
        /// Moved item with the new position.
        /// </summary>
        public readonly GridItem Item;

        /// <summary>
        /// Origin before the move.
        /// </summary>
        public readonly Vector2Int PreviousOrigin;

        /// <summary>
        /// Rotation before the move.
        /// </summary>
        public readonly int PreviousRotation;
    }
}
