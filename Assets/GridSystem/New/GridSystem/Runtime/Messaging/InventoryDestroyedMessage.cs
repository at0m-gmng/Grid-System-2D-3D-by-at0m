namespace GridSystem.Runtime.Messaging
{
    using Core.Inventories;

    /// <summary>
    /// Published before an inventory is cleared and disposed.
    /// </summary>
    public readonly struct InventoryDestroyedMessage
    {
        public InventoryDestroyedMessage(GridInventory inventory)
        {
            Inventory = inventory;
        }

        /// <summary>
        /// Destroyed inventory.
        /// </summary>
        public readonly GridInventory Inventory;
    }
}
