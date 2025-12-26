namespace GridSystem.MergeSystem
{
    using System;
    using InventorySystem.Inventory.Data;
    using InventorySystem.Inventory.Enums;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Inventory/MergeConfig", fileName = "MergeConfig")]
    public sealed class MergeConfig : ScriptableObject
    {
        [field: SerializeField] public MergeRule[] Rules { get; private set; }
    }

    [Serializable]
    public sealed class MergeRule
    {
        [field: SerializeField] public ItemType Type { get; private set; }
        [field: SerializeField] public int Level { get; private set; }
        [field: SerializeField] public ItemData ResultItem { get; private set; }
    }
}