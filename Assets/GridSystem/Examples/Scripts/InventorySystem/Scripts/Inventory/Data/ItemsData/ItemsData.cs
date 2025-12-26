namespace GridSystem.InventorySystem.Inventory.Data
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ItemsData : ScriptableObject
    {
        [field: SerializeField] public List<ItemData> Item { get; private set; }
    }
}