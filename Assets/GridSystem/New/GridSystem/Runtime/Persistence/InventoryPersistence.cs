namespace GridSystem.Runtime.Persistence
{
    using Core.Inventories;
    using Core.Items;
    using Definitions;
    using Inventories;
    using UnityEngine;

    /// <summary>
    /// JSON inventory persistence. A save is restored as a whole: when any stored item is unknown
    /// or no longer fits, the inventory is left empty and the save is dropped, so the caller refills defaults.
    /// </summary>
    public sealed class InventoryPersistence : IInventoryPersistence
    {
        public InventoryPersistence(ISaveStorage storage, IItemDefinitionCatalog catalog, IGridItemFactory factory)
        {
            _storage = storage;
            _catalog = catalog;
            _factory = factory;
        }

        private readonly ISaveStorage _storage;
        private readonly IItemDefinitionCatalog _catalog;
        private readonly IGridItemFactory _factory;
        private readonly InventorySnapshot _snapshot = new();

        /// <summary>
        /// Stores current content.
        /// </summary>
        public void Save(GridInventory inventory)
        {
            _snapshot.Items.Clear();
            foreach (GridItem item in inventory.Items)
            {
                _snapshot.Items.Add(new ItemSnapshot(item.Definition.Id, item.Origin, item.Rotation));
            }

            _storage.Write(inventory.Key, JsonUtility.ToJson(_snapshot));
        }

        /// <summary>
        /// Replaces inventory content with stored items. Returns false and clears storage when nothing is stored
        /// or when any snapshot item cannot be restored (unknown definition or does not fit).
        /// </summary>
        public bool TryLoad(GridInventory inventory)
        {
            if (!_storage.TryRead(inventory.Key, out string json))
            {
                return false;
            }

            inventory.Clear();
            _snapshot.Items.Clear();
            JsonUtility.FromJsonOverwrite(json, _snapshot);
            bool allPlaced = true;
            for (int i = 0; i < _snapshot.Items.Count; i++)
            {
                ItemSnapshot snapshot = _snapshot.Items[i];
                if (!_catalog.TryGet(snapshot.DefinitionId, out IGridItemDefinition definition)
                    || !inventory.TryAdd(_factory.Create(definition), snapshot.Origin, snapshot.Rotation))
                {
                    allPlaced = false;
                }
            }

            if (!allPlaced)
            {
                inventory.Clear();
                _storage.Delete(inventory.Key);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes stored content.
        /// </summary>
        public void Delete(GridInventory inventory) => _storage.Delete(inventory.Key);
    }
}
