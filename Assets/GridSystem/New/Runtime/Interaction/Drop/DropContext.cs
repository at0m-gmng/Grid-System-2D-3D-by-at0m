namespace GridSystem.Runtime.Interaction.Drop
{
    using Core.Inventories;
    using Core.Items;
    using UnityEngine;

    /// <summary>
    /// Drop request of a dragged item onto a surface cell.
    /// </summary>
    public readonly struct DropContext
    {
        public DropContext(GridItem item, GridInventory source, GridInventory target, Vector2Int cell, int rotation, GridItem targetItem)
        {
            Item = item;
            Source = source;
            Target = target;
            Cell = cell;
            Rotation = rotation;
            TargetItem = targetItem;
        }

        /// <summary>
        /// Dragged item.
        /// </summary>
        public readonly GridItem Item;

        /// <summary>
        /// Inventory containing the dragged item.
        /// </summary>
        public readonly GridInventory Source;

        /// <summary>
        /// Inventory under the pointer.
        /// </summary>
        public readonly GridInventory Target;

        /// <summary>
        /// Cell under the pointer, desired pivot position.
        /// </summary>
        public readonly Vector2Int Cell;

        /// <summary>
        /// Desired rotation.
        /// </summary>
        public readonly int Rotation;

        /// <summary>
        /// Item occupying the cell or null.
        /// </summary>
        public readonly GridItem TargetItem;
    }
}
