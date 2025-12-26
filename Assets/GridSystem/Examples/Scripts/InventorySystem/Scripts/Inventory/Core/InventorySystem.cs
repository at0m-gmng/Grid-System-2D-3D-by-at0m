namespace GridSystem.InventorySystem.Inventory.Core
{
    using System;
    using System.Collections.Generic;
    using Configs;
    using Data;
    using Enums;
    using GridSystem.Core;
    using UnityEngine;
    using Grid = GridSystem.Core.Grid;

    [Serializable]
    public sealed class InventorySystem
    {
        public InventorySystem(InventoryConfig inventoryConfig)
        {
            _inventoryConfig = inventoryConfig;
            _inventory = new Grid { Rows = new List<ColumnList>(inventoryConfig.Rows) };

            for (int i = 0; i < InventoryConfig.Rows; i++)
            {
                ColumnList matrixRow = new ColumnList { Columns = new List<int>(InventoryConfig.Columns) };

                for (int j = 0; j < InventoryConfig.Columns; j++)
                {
                    matrixRow.Columns.Add((int)CellType.Empty);
                }

                _inventory.Rows.Add(matrixRow);
            }
        }

        private readonly InventoryConfig _inventoryConfig;

        public InventoryConfig InventoryConfig => _inventoryConfig;
        public IReadOnlyList<PlacementItem> PlacementItems => _placementItems;

        private List<PlacementItem> _placementItems = new();
        private Grid _inventory;

        public bool TryReleasePlacement(PlacementItem placement)
        {
            PlacementItem placementItem = _placementItems.Find(x => x.ID == placement.ID && x.PlacementCenter == placement.PlacementCenter);

            if (placementItem != null)
            {
                Vector2Int shapeCenter = placementItem.Shape.GetItemCenter();
                int startItemRow = placementItem.PlacementCenter.x - shapeCenter.x;
                int startItemCol = placementItem.PlacementCenter.y - shapeCenter.y;

                if (AreCellsInBounds(startItemRow, startItemCol, placementItem.Shape))
                {
                    ReleaseCells(placementItem.PlacementCells);
                    _placementItems.Remove(placementItem);

                    return true;
                }
            }

            return false;
        }

        public bool TryRestorePlacement(PlacementItem placement)
        {
            if (placement != null)
            {
                Vector2Int shapeCenter = placement.Shape.GetItemCenter();
                int startItemRow = placement.PlacementCenter.x - shapeCenter.x;
                int startItemCol = placement.PlacementCenter.y - shapeCenter.y;

                if (AreCellsInBounds(startItemRow, startItemCol, placement.Shape))
                {
                    for (int i = 0; i < placement.Shape.Rows.Count; i++)
                    {
                        for (int j = 0; j < placement.Shape.Rows[i].Columns.Count; j++)
                        {
                            if (placement.Shape.Rows[i].Columns[j] != (int)CellType.Empty)
                            {
                                int nextRow = startItemRow + i;
                                int nextCol = startItemCol + j;
                                if (_inventory.Rows[nextRow].Columns[nextCol] != (int)CellType.Empty)
                                {
                                    return false;
                                }
                            }
                        }
                    }

                    OccupyCells(placement.Shape, placement.PlacementCells, startItemRow, startItemCol);
                    _placementItems.Add(placement);
                    return true;
                }

                return false;
            }

            return false;
        }

        public void TryPlaceItem(ItemType ID, Grid shape, int rotationIndex, int centerRow, int centerCol, int posRow, int posCol, out PlacementItem placementItem)
        {
            List<Vector2Int> busyCells = new List<Vector2Int>();
            OccupyCells(shape, busyCells, posRow, posCol);
            PlacementItem newPlacement = new PlacementItem
            {
                ID = ID,
                PlacementCenter = new Vector2Int(centerRow, centerCol),
                PlacementCells = busyCells,
                Shape = Grid.Clone(shape),
                RotationIndex = rotationIndex
            };
            _placementItems.Add(newPlacement);
            placementItem = newPlacement;
        }

        public bool IsAvailablePlacement(int row, int col) => _inventory.Rows[row].Columns[col] == (int)CellType.Empty;

        public bool IsAvailablePlacement(int row, int col, Grid shape)
        {
            for (int i = 0; i < shape.Rows.Count; i++)
            {
                for (int j = 0; j < shape.Rows[i].Columns.Count; j++)
                {
                    if (shape.Rows[i].Columns[j] != (int)CellType.Empty)
                    {
                        int nextRow = row + i;
                        int nextCol = col + j;

                        if ((nextRow < 0 || nextRow >= _inventory.Rows.Count) ||
                            (nextCol < 0 || nextCol >= _inventory.Rows[nextRow].Columns.Count) ||
                            (_inventory.Rows[nextRow].Columns[nextCol] != (int)CellType.Empty))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void OccupyCells(Grid shape, List<Vector2Int> placementCells, int posRow, int posCol)
        {
            if (placementCells != null)
            {
                placementCells.Clear();
                for (int i = 0; i < shape.Rows.Count; i++)
                {
                    for (int j = 0; j < shape.Rows[i].Columns.Count; j++)
                    {
                        if (shape.Rows[i].Columns[j] != (int)CellType.Empty)
                        {
                            _inventory.Rows[posRow + i].Columns[posCol + j] = (int)CellType.Busy;
                            placementCells.Add(new Vector2Int(posRow + i, posCol + j));
                        }
                    }
                }
            }
        }

        public void ReleaseCells(List<Vector2Int> placement)
        {
            if (placement != null)
            {
                for (int k = 0; k < placement.Count; k++)
                {
                    _inventory.Rows[placement[k].x].Columns[placement[k].y] = (int)CellType.Empty;
                }
            }
        }

        public bool AreCellsInBounds(int startRow, int startCol, Grid shape)
        {
            if (startRow >= 0 && startRow + shape.Rows.Count <= _inventory.Rows.Count)
            {
                for (int i = 0; i < shape.Rows.Count; i++)
                {
                    int nextRow = startRow + i;
                    if ((nextRow < 0 || nextRow >= _inventory.Rows.Count) ||
                        (startCol < 0 || startCol + shape.Rows[i].Columns.Count > _inventory.Rows[nextRow].Columns.Count))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }

    [Serializable]
    public sealed class InventorySaveData
    {
        public List<PlacementItem> Items = new();
    }
}