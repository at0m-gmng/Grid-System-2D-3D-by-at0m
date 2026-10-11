namespace GridSystem.Runtime.Messaging
{
    using Core.Inventories;
    using Core.Items;

    /// <summary>
    /// Published when the user starts dragging an item.
    /// </summary>
    public readonly struct ItemDragStartedMessage
    {
        public ItemDragStartedMessage(GridInventory source, GridItem item)
        {
            Source = source;
            Item = item;
        }

        /// <summary>
        /// Inventory the item is dragged from.
        /// </summary>
        public readonly GridInventory Source;

        /// <summary>
        /// Dragged item.
        /// </summary>
        public readonly GridItem Item;
    }
}
