namespace GridSystem.InventorySystem.Inventory.Data
{
    using System;

    [Serializable]
    public sealed class BaseItemView
    {
        public string Name;
        public float AttackSpeed;
        public float Damage;
        public float Range;
    }
}