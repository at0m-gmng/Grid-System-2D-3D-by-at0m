namespace GridSystem.InventoryPresenters
{
    using System.Collections.Generic;
    using DragDropSystem.View;
    using InventorySystem.Inventory.Core;
    using InventorySystem.Inventory.Data;
    using UnityEngine;
    using Grid = Core.Grid;

    public abstract class AbstractInventoryPresenter<T> : IInventoryPresenter where T : Transform
    {
        public AbstractInventoryPresenter(T itemParent, List<List<T>> cellObjects, InventorySystem inventorySystem)
        {
            _itemParent = itemParent;
            _cellObjects = cellObjects;
            _inventorySystem = inventorySystem;
        }

        protected T _itemParent;
        protected List<List<T>> _cellObjects;
        protected InventorySystem _inventorySystem;

        protected const int MAX_ROTATION_COUNT = 4;

        #region PUBLIC_REGION

        public virtual bool TryAutoPlaceItem(IPoolableDraggableObject itemView)
        {
            for (int i = 0; i < _cellObjects.Count; i++)
            {
                for (int j = 0; j < _cellObjects[i].Count; j++)
                {
                    Vector3 cellPos = _cellObjects[i][j].position;

                    for (int k = 0; k < MAX_ROTATION_COUNT; k++)
                    {
                        BaseItem rotated = itemView.ItemData.GetRotation(k);
                        if (IsAvailablePlaceByCenter(rotated.Grid, cellPos))
                        {
                            itemView.ItemData = rotated;
                            if (TryPlaceItem(itemView, cellPos, rotated.Grid))
                            {
                                itemView.ApplyRotation(k);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public virtual bool TryPlaceItem(IPoolableDraggableObject itemView) => TryPlaceItem(itemView, itemView.Transform.position);

        public abstract bool IsAvailablePlaceByCenter(Grid shape, Vector3 worldPosition);

        public virtual bool TryReleasePlacement(PlacementItem placement) => _inventorySystem.TryReleasePlacement(placement);

        public virtual bool TryRestorePlacement(PlacementItem placement) => _inventorySystem.TryRestorePlacement(placement);

        public virtual void TryRestorePlacement(IPoolableDraggableObject itemView)
        {
            int centerRow = itemView.PlacementItem.PlacementCenter.x;
            int centerCol = itemView.PlacementItem.PlacementCenter.y;

            if (centerRow >= 0 && centerRow < _cellObjects.Count &&
                centerCol >= 0 && centerCol < _cellObjects[centerRow].Count)
            {
                itemView.Transform.position = _cellObjects[centerRow][centerCol].transform.position;
                itemView.Transform.SetParent(_itemParent);
            }
        }

        #endregion

        #region PRIVATE_REGION

        protected abstract bool TryPlaceItem(IPoolableDraggableObject itemView, Vector3 position, Grid shape = default);

        protected abstract bool TryGetPlacementByCenter(Grid shape, Vector3 position, out int startRow, out int startCol, out int centerRow, out int centerCol);

        #endregion
    }
}