namespace GridSystem.Runtime.Messaging
{
    using Core.Inventories;

    /// <summary>
    /// Published after an item is moved inside any inventory.
    /// </summary>
    public readonly struct ItemMovedMessage
    {
        public ItemMovedMessage(GridInventory inventory, ItemMovement movement)
        {
            Inventory = inventory;
            Movement = movement;
        }

        /// <summary>
        /// Owning inventory.
        /// </summary>
        public readonly GridInventory Inventory;

        /// <summary>
        /// Move data.
        /// </summary>
        public readonly ItemMovement Movement;
    }
}
