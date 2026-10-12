namespace GridSystem.InventoryPresenters
{
    using DragDropSystem.View;
    using InventorySystem.Inventory.Data;
    using UnityEngine;
    using Grid = Core.Grid;

    public interface IInventoryPresenter
    {
        public bool TryAutoPlaceItem(IPoolableDraggableObject itemView);
        public bool TryPlaceItem(IPoolableDraggableObject itemView);
        public void TryRestorePlacement(IPoolableDraggableObject itemView);
        public bool IsAvailablePlaceByCenter(Grid shape, Vector3 worldPosition);
        public bool TryReleasePlacement(PlacementItem placement);
        public bool TryRestorePlacement(PlacementItem placement);
    }
}