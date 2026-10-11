namespace GridSystem.Runtime.Messaging
{
    using Core.Inventories;
    using Core.Items;
    using Surfaces;

    /// <summary>
    /// Published when the user starts dragging an item.
    /// </summary>
    public readonly struct ItemDragStartedMessage
    {
        public ItemDragStartedMessage(GridItem item, GridInventory source, GridSurface sourceSurface)
        {
            Item = item;
            Source = source;
            SourceSurface = sourceSurface;
        }

        /// <summary>
        /// Dragged item.
        /// </summary>
        public readonly GridItem Item;

        /// <summary>
        /// Inventory containing the item.
        /// </summary>
        public readonly GridInventory Source;

        /// <summary>
        /// Surface the item is dragged from.
        /// </summary>
        public readonly GridSurface SourceSurface;
    }
}
