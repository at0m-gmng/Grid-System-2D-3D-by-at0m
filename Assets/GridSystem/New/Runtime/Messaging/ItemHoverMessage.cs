namespace GridSystem.Runtime.Messaging
{
    using Core.Inventories;
    using Core.Items;

    /// <summary>
    /// Published when the hovered item changes. Item is null when nothing is hovered.
    /// </summary>
    public readonly struct ItemHoverMessage
    {
        public ItemHoverMessage(GridInventory inventory, GridItem item)
        {
            Inventory = inventory;
            Item = item;
        }

        /// <summary>
        /// Inventory of the hovered item.
        /// </summary>
        public readonly GridInventory Inventory;

        /// <summary>
        /// Hovered item or null.
        /// </summary>
        public readonly GridItem Item;
    }
}
