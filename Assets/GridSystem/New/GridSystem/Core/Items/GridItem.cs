namespace GridSystem.Core.Items
{
    using UnityEngine;

    /// <summary>
    /// Item instance. Position is changed only by the owning inventory.
    /// </summary>
    public sealed class GridItem
    {
        public GridItem(int id, IGridItemDefinition definition)
        {
            Id = id;
            Definition = definition;
        }

        /// <summary>
        /// Runtime unique identifier, never equals <see cref="Model.GridModel.EMPTY"/>.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Static item data.
        /// </summary>
        public IGridItemDefinition Definition { get; }

        /// <summary>
        /// Id of the inventory containing the item or <see cref="Model.GridModel.EMPTY"/> when the item is not placed.
        /// </summary>
        public int InventoryId { get; internal set; }

        /// <summary>
        /// Cell of the pivot inside the owning inventory.
        /// </summary>
        public Vector2Int Origin { get; internal set; }

        /// <summary>
        /// Clockwise quarter turns in range [0, 3].
        /// </summary>
        public int Rotation { get; internal set; }
    }
}
