namespace GridSystem.Factories
{
    using InventoryPresenters;
    using InventorySystem.Inventory.Data;
    using UnityEngine;

    public struct DraggableData
    {
        public DraggableData(ItemData itemData, Transform tempParent, IInventoryPresenter inventoryView = null, PlacementItem placementItem = null)
        {
            ItemData = itemData;
            InventoryView = inventoryView;
            PlacementItem = placementItem;
            TempParent = tempParent;
        }

        public readonly ItemData ItemData;
        public readonly IInventoryPresenter InventoryView;
        public readonly PlacementItem PlacementItem;
        public readonly Transform TempParent;
    }
}