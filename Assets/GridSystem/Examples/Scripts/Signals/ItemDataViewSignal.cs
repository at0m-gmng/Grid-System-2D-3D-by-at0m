namespace GridSystem.Signals
{
    using InventorySystem.Inventory.Data;
    using UnityEngine;

    public sealed class ItemDataViewSignal
    {
        public BaseItemView ItemDataView = default;
        public Transform ItemTransform = default;
    }
}