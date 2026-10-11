namespace GridSystem.Runtime.Messaging
{
    using Core.Inventories;
    using Core.Items;
    using Surfaces;

    /// <summary>
    /// Published after the drag is finished and the accepted drop is applied.
    /// </summary>
    public readonly struct ItemDragEndedMessage
    {
        public ItemDragEndedMessage(GridItem item, GridInventory source, GridSurface sourceSurface, GridSurface surface, bool isDropped)
        {
            Item = item;
            Source = source;
            SourceSurface = sourceSurface;
            Surface = surface;
            IsDropped = isDropped;
        }

        /// <summary>
        /// Dragged item.
        /// </summary>
        public readonly GridItem Item;

        /// <summary>
        /// Inventory the item was dragged from.
        /// </summary>
        public readonly GridInventory Source;

        /// <summary>
        /// Surface the item was dragged from.
        /// </summary>
        public readonly GridSurface SourceSurface;

        /// <summary>
        /// Last hovered surface.
        /// </summary>
        public readonly GridSurface Surface;

        /// <summary>
        /// True when a drop handler accepted the item.
        /// </summary>
        public readonly bool IsDropped;
    }
}
