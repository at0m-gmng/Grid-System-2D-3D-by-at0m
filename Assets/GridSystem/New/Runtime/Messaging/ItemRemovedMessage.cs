namespace GridSystem.Runtime.Messaging
{
    using Core.Inventories;
    using Core.Items;

    /// <summary>
    /// Published after an item is removed from any inventory.
    /// </summary>
    public readonly struct ItemRemovedMessage
    {
        public ItemRemovedMessage(GridInventory inventory, GridItem item)
        {
            Inventory = inventory;
            Item = item;
        }

        /// <summary>
        /// Source inventory.
        /// </summary>
        public readonly GridInventory Inventory;

        /// <summary>
        /// Removed item.
        /// </summary>
        public readonly GridItem Item;
    }
}
