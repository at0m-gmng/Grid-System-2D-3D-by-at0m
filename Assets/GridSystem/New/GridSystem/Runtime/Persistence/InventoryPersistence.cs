namespace GridSystem.Runtime.Persistence
{
    using Core.Inventories;
    using Core.Items;
    using Definitions;
    using Inventories;
    using UnityEngine;

    /// <summary>
    /// JSON inventory persistence. Unknown or not fitting items are skipped on load.
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
        /// Replaces inventory content with stored items. Returns false when nothing is stored or the save does not match current definitions
        /// and grid; such a save is deleted and the inventory is left empty.
        /// </summary>
        public bool TryLoad(GridInventory inventory)
        {
            bool result = _storage.TryRead(inventory.Key, out string json);
            if (result)
            {
                inventory.Clear();
                _snapshot.Items.Clear();
                JsonUtility.FromJsonOverwrite(json, _snapshot);
                for (int i = 0; result && i < _snapshot.Items.Count; i++)
                {
                    ItemSnapshot snapshot = _snapshot.Items[i];
                    result = _catalog.TryGet(snapshot.DefinitionId, out IGridItemDefinition definition)
                        && inventory.TryAdd(_factory.Create(definition), snapshot.Origin, snapshot.Rotation);
                }

                if (!result)
                {
                    inventory.Clear();
                    _storage.Delete(inventory.Key);
                }
            }

            return result;
        }

        public void Delete(GridInventory inventory) => _storage.Delete(inventory.Key);
    }
}
