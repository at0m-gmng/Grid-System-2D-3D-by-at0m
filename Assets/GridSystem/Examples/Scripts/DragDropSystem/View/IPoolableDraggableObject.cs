namespace GridSystem.DragDropSystem.View
{
    using InventoryPresenters;
    using InventorySystem.Inventory.Data;
    using UnityEngine;

    public interface IPoolableDraggableObject
    {
        public Transform Transform { get; }
        public PlacementItem PlacementItem { get; set; }
        public IInventoryPresenter InventoryView { get; }
        public BaseItem ItemData { get; set; }
        public BaseItemView ItemDataView { get; }
        public int ID { get; }
        public void ApplyRotation(int rotationCount);
        public void ReturnToPool();
    }
}