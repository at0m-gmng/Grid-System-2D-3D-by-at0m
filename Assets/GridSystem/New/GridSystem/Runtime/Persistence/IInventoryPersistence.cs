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
        /// Replaces inventory content with stored items. Returns false when nothing is stored or the save does not match current definitions
        /// and grid; such a save is deleted and the inventory is left empty.
        /// </summary>
        public bool TryLoad(GridInventory inventory);

        /// <summary>
        /// Removes stored content.
        /// </summary>
        public void Delete(GridInventory inventory);
    }
}
