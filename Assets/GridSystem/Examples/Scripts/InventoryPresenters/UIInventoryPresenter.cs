namespace GridSystem.InventoryPresenters
{
    using System.Collections.Generic;
    using DragDropSystem.View;
    using InventorySystem.Inventory.Core;
    using InventorySystem.Inventory.Data;
    using UnityEngine;
    using UnityEngine.UI;
    using Grid = Core.Grid;

    public sealed class UIInventoryPresenter : AbstractInventoryPresenter<RectTransform>
    {
        public UIInventoryPresenter(GridLayoutGroup gridLayoutGroup, RectTransform itemParent, List<List<RectTransform>> cellObjects, InventorySystem inventorySystem)
            : base(itemParent, cellObjects, inventorySystem)
        {
            _gridLayout = gridLayoutGroup;
            _itemParent = itemParent;
            _cellObjects = cellObjects;
            _inventorySystem = inventorySystem;
        }

        private readonly GridLayoutGroup _gridLayout;

        private float CELL_OFFSET = 0.025f;

        #region PUBLIC_REGION

        public override bool IsAvailablePlaceByCenter(Grid shape, Vector3 worldPosition)
        {
            Vector2 localItemPos = _gridLayout.transform.InverseTransformPoint(worldPosition);

            int targetRow = -1, targetCol = -1;
            float minDistSq = CELL_OFFSET * (_gridLayout.cellSize.x * _gridLayout.cellSize.x + _gridLayout.cellSize.y * _gridLayout.cellSize.y);

            for (int i = 0; i < _cellObjects.Count; i++)
            {
                for (int j = 0; j < _cellObjects[i].Count; j++)
                {
                    Vector2 localCellPos = _gridLayout.transform.InverseTransformPoint(_cellObjects[i][j].transform.position);
                    float dx = localItemPos.x - localCellPos.x;
                    float dy = localItemPos.y - localCellPos.y;
                    float distSq = dx * dx + dy * dy;
                    if (distSq < minDistSq)
                    {
                        minDistSq = distSq;
                        targetRow = i;
                        targetCol = j;
                    }
                }
            }

            if ((targetRow != -1 && targetCol != -1) && _inventorySystem.IsAvailablePlacement(targetRow, targetCol))
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
                    centerRow, centerCol, startRow, startCol, out PlacementItem newPlacement);

                itemView.Transform.SetParent(_itemParent);
                itemView.Transform.localPosition = _itemParent.InverseTransformPoint(_cellObjects[centerRow][centerCol].transform.position);
                itemView.PlacementItem = newPlacement;

                return true;
            }

            return false;
        }

        protected override bool TryGetPlacementByCenter(Grid shape, Vector3 position, out int startItemRow, out int startItemCol, out int centerRow, out int centerCol)
        {
            startItemRow = startItemCol = centerRow = centerCol = -1;
            Vector2 localItemPos = _gridLayout.transform.InverseTransformPoint(position);
            float minDistSq = float.MaxValue;

            for (int i = 0; i < _cellObjects.Count; i++)
            {
                for (int j = 0; j < _cellObjects[i].Count; j++)
                {
                    Vector2 localCellPos = _gridLayout.transform.InverseTransformPoint(_cellObjects[i][j].transform.position);
                    float dx = localItemPos.x - localCellPos.x;
                    float dy = localItemPos.y - localCellPos.y;
                    float d2 = dx * dx + dy * dy;
                    if (d2 < minDistSq)
                    {
                        minDistSq = d2;
                        centerRow = i;
                        centerCol = j;
                    }
                }
            }

            if (centerRow >= 0)
            {
                Vector2Int shapeCenter = shape.GetItemCenter();
                startItemRow = centerRow - shapeCenter.x;
                startItemCol = centerCol - shapeCenter.y;

                return _inventorySystem.IsAvailablePlacement(startItemRow, startItemCol, shape);
            }

            return false;
        }

        #endregion
    }
}