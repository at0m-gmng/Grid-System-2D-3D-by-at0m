namespace GridSystem.Runtime.Messaging
{
    using Core.Inventories;
    using Core.Items;

    /// <summary>
    /// Published after an item is placed into any inventory.
    /// </summary>
    public readonly struct ItemAddedMessage
    {
        public ItemAddedMessage(GridInventory inventory, GridItem item)
        {
            Inventory = inventory;
            Item = item;
        }

        /// <summary>
        /// Target inventory.
        /// </summary>
        public readonly GridInventory Inventory;

        /// <summary>
        /// Placed item.
        /// </summary>
        public readonly GridItem Item;
    }
}
