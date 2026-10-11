namespace GridSystem.Runtime.Messaging
{
    using Core.Inventories;
    using Core.Items;

    /// <summary>
    /// Published when the user releases a dragged item.
    /// </summary>
    public readonly struct ItemDragEndedMessage
    {
        public ItemDragEndedMessage(GridInventory source, GridItem item, bool isDropped)
        {
            Source = source;
            Item = item;
            IsDropped = isDropped;
        }

        /// <summary>
        /// Inventory the item was dragged from.
        /// </summary>
        public readonly GridInventory Source;

        /// <summary>
        /// Dragged item.
        /// </summary>
        public readonly GridItem Item;

        /// <summary>
        /// True when a drop handler accepted the item.
        /// </summary>
        public readonly bool IsDropped;
    }
}
