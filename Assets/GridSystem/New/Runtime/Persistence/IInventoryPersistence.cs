namespace GridSystem.Runtime.Persistence
{
    using Core.Inventories;

    /// <summary>
    /// Saves and restores inventory content by inventory key.
    /// </summary>
    public interface IInventoryPersistence
    {
        /// <summary>
        /// Stores current content.
        /// </summary>
        public void Save(GridInventory inventory);

        /// <summary>
        /// Adds stored items into the inventory. Returns false when nothing is stored.
        /// </summary>
        public bool TryLoad(GridInventory inventory);

        /// <summary>
        /// Removes stored content.
        /// </summary>
        public void Delete(GridInventory inventory);
    }
}
