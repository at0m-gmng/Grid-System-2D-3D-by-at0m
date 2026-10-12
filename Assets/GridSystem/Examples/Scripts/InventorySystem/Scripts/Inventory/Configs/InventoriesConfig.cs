namespace GridSystem.InventorySystem.Inventory.Configs
{
    using System;
    using System.Collections.Generic;
    using Enums;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Inventory/Configs/InventoriesConfig", fileName = "InventoriesConfig")]
    public sealed class InventoriesConfig : ScriptableObject
    {
        [field: SerializeField] public List<InventoryGroup> Inventories { get; private set; }

        public bool TryGetInventoryConfig(InventoryType id, out InventoryConfig config)
        {
            InventoryGroup group = Inventories.Find(x => x.ID == id);
            config = group?.Inventory;
            return config != null;
        }
    }

    [Serializable]
    public sealed class InventoryGroup
    {
        public InventoryType ID;
        public InventoryConfig Inventory;
    }
}