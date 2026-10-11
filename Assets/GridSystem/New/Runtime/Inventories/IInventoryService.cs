namespace GridSystem.Runtime.Inventories
{
    using Core.Inventories;
    using Definitions;

    /// <summary>
    /// Creates and tracks inventories. Relays their changes to the message bus.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Creates an empty inventory.
        /// </summary>
        public GridInventory Create(GridDefinition definition, string key);

        /// <summary>
        /// Finds inventory by runtime id.
        /// </summary>
        public bool TryGet(int id, out GridInventory inventory);

        /// <summary>
        /// Clears and forgets the inventory.
        /// </summary>
        public void Destroy(GridInventory inventory);
    }
}
