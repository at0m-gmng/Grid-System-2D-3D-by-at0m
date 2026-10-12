namespace GridSystem.InventoryPresenters
{
    using System.Collections.Generic;
    using DragDropSystem.View;
    using InventorySystem.Inventory.Core;
    using InventorySystem.Inventory.Data;
    using UnityEngine;
    using Grid = Core.Grid;

    public sealed class WorldInventoryPresenter : AbstractInventoryPresenter<Transform>
    {
        public WorldInventoryPresenter(Transform itemParent, List<List<Transform>> cellObjects, InventorySystem inventorySystem)
            : base(itemParent, cellObjects, inventorySystem)
        {
            _itemParent = itemParent;
            _cellObjects = cellObjects;
            _inventorySystem = inventorySystem;
        }

        private const float MAX_PLACEMENT_DISTANCE = 1.5f;

        #region PUBLIC_REGION

        public override bool IsAvailablePlaceByCenter(Grid shape, Vector3 worldPosition)
        {
            int targetRow = -1, targetCol = -1;
            float minDistSq = float.MaxValue;

            for (int i = 0; i < _cellObjects.Count; i++)
            {
                for (int j = 0; j < _cellObjects[i].Count; j++)
                {
                    Vector3 cellPos = _cellObjects[i][j].position;
                    float dx = worldPosition.x - cellPos.x;
                    float dz = worldPosition.z - cellPos.z;
                    float distSq = dx * dx + dz * dz;

                    if (distSq < minDistSq)
                    {
                        minDistSq = distSq;
                        targetRow = i;
                        targetCol = j;
                    }
                }
            }

            if (minDistSq > MAX_PLACEMENT_DISTANCE * MAX_PLACEMENT_DISTANCE)
                return false;

            if (targetRow != -1 && targetCol != -1 && _inventorySystem.IsAvailablePlacement(targetRow, targetCol))
            {
                Vector2Int shapeCenter = shape.GetItemCenter();
                int startRow = targetRow - shapeCenter.x;
                int startCol = targetCol - shapeCenter.y;
                return _inventorySystem.IsAvailablePlacement(startRow, startCol, shape);
            }

            return false;
        }

        #endregion

        #region PRIVATE_REGION

        protected override bool TryPlaceItem(IPoolableDraggableObject itemView, Vector3 position, Grid shape = default)
        {
            if (shape.Rows == null || shape.Rows.Count == 0)
            {
                shape = Grid.Clone(itemView.ItemData.Grid);
            }

            if (TryGetPlacementByCenter(shape, position, out int startRow, out int startCol, out int centerRow, out int centerCol))
            {
                _inventorySystem.TryPlaceItem(itemView.ItemData.Type, shape, itemView.PlacementItem.RotationIndex,
                    centerRow, centerCol, startRow, startCol, out PlacementItem newPlacement
                );
                Vector3 targetPos = _cellObjects[centerRow][centerCol].position;
                itemView.Transform.position = targetPos;
                itemView.Transform.SetParent(_itemParent);
                itemView.PlacementItem = newPlacement;
                return true;
            }

            return false;
        }

        protected override bool TryGetPlacementByCenter(Grid shape, Vector3 position, out int startRow, out int startCol, out int centerRow, out int centerCol)
        {
            startRow = startCol = centerRow = centerCol = -1;
            float minDistSq = float.MaxValue;

            for (int i = 0; i < _cellObjects.Count; i++)
            {
                for (int j = 0; j < _cellObjects[i].Count; j++)
                {
                    Vector3 cellPos = _cellObjects[i][j].position;
                    float dx = position.x - cellPos.x;
                    float dz = position.z - cellPos.z;
                    float distSq = dx * dx + dz * dz;

                    if (distSq < minDistSq)
                    {
                        minDistSq = distSq;
                        centerRow = i;
                        centerCol = j;
                    }
                }
            }

            if (minDistSq > MAX_PLACEMENT_DISTANCE * MAX_PLACEMENT_DISTANCE)
                return false;

            if (centerRow >= 0)
            {
                Vector2Int shapeCenter = shape.GetItemCenter();
                startRow = centerRow - shapeCenter.x;
                startCol = centerCol - shapeCenter.y;
                return _inventorySystem.IsAvailablePlacement(startRow, startCol, shape);
            }

            return false;
        }

        #endregion
    }
}