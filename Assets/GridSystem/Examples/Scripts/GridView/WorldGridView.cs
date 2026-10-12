namespace GridSystem.GridView
{
    using System.Collections.Generic;
    using Core;
    using InventoryPresenters;
    using InventorySystem.Inventory.Core;
    using InventorySystem.Inventory.Data;
    using InventorySystem.Inventory.Enums;
    using UnityEngine;
    using Grid = Core.Grid;

    public sealed class WorldGridView : AbstractGridView<Transform, WorldInventoryPresenter>
    {
        [SerializeField]
        private Transform cellsParent;

        [SerializeField]
        private int _spacing = 1;

        protected override void Initialize(int rows, int columns)
            => UpdateGrid(rows, columns);

        protected override WorldInventoryPresenter CreatePresenter(InventorySystemData inventorySystem)
            => new WorldInventoryPresenter(itemParent, cellObjects, inventorySystem.InventorySystem);

        protected override void UpdateGrid(int countX, int countZ)
        {
            while (cellObjects.Count > countZ)
            {
                foreach (var cell in cellObjects[^1]) Destroy(cell.gameObject);
                cellObjects.RemoveAt(cellObjects.Count - 1);
            }

            while (cellObjects.Count < countZ)
            {
                cellObjects.Add(new List<Transform>());
            }

            float offsetX = (countX - 1) * _spacing / 2f;
            float offsetZ = (countZ - 1) * _spacing / 2f;
            Vector3 startOffset = new Vector3(-offsetX, 0, offsetZ);

            for (int i = 0; i < countZ; i++)
            {
                while (cellObjects[i].Count > countX)
                {
                    Destroy(cellObjects[i][^1].gameObject);
                    cellObjects[i].RemoveAt(cellObjects[i].Count - 1);
                }

                for (int j = 0; j < countX; j++)
                {
                    float zPosition = -i * _spacing;
                    Vector3 position = itemParent.transform.position + startOffset + new Vector3(j * _spacing, 0, zPosition);

                    if (j < cellObjects[i].Count)
                    {
                        cellObjects[i][j].position = position;
                    }
                    else
                    {
                        Transform cell = Instantiate(cellPrefab, cellsParent);
                        cell.name = $"{cellPrefab.name}_{i}_{j}";
                        cell.position = position;
                        cellObjects[i].Add(cell);
                    }
                }
            }
        }

        protected override void CreateDefaultItems(InventorySystemData inventorySystem)
        {
            if (_spawnData.PlacementItems.Count == 0)
            {
                List<Vector2Int> placementItems_1 = new List<Vector2Int>();
                placementItems_1.Add(Vector2Int.zero);

                Grid grid_1 = new Grid();
                grid_1.Rows = new List<ColumnList>();
                ColumnList columnList_1 = new ColumnList();
                columnList_1.Columns = new List<int>();
                columnList_1.Columns.Add(2);
                grid_1.Rows.Add(columnList_1);

                PlacementItem first = new PlacementItem
                {
                    ID = ItemType.LaserTower,
                    PlacementCenter = Vector2Int.zero,
                    PlacementCells = new List<Vector2Int>(placementItems_1),
                    Shape = grid_1,
                    RotationIndex = 0
                };

                List<Vector2Int> placementItems_2 = new List<Vector2Int>();
                placementItems_2.Add(new Vector2Int(0, 1));
                placementItems_2.Add(new Vector2Int(0, 2));
                placementItems_2.Add(new Vector2Int(1, 1));

                Grid grid_2 = new Grid();
                grid_2.Rows = new List<ColumnList>();

                ColumnList columnList_2 = new ColumnList { Columns = new List<int> { 2, 1 } };
                ColumnList columnList_3 = new ColumnList { Columns = new List<int> { 1, 0 } };

                grid_2.Rows.Add(columnList_2);
                grid_2.Rows.Add(columnList_3);

                PlacementItem second = new PlacementItem
                {
                    ID = ItemType.Wall,
                    PlacementCenter = new Vector2Int(0, 1),
                    PlacementCells = new List<Vector2Int>(placementItems_2),
                    Shape = grid_2,
                    RotationIndex = 0
                };

                List<Vector2Int> placementItems_3 = new List<Vector2Int>();
                placementItems_1.Add(Vector2Int.zero);

                Grid grid_3 = new Grid();
                grid_3.Rows = new List<ColumnList>();
                ColumnList columnList_4 = new ColumnList();
                columnList_4.Columns = new List<int>();
                columnList_4.Columns.Add(2);
                grid_3.Rows.Add(columnList_4);

                PlacementItem third = new PlacementItem
                {
                    ID = ItemType.LaserTower,
                    PlacementCenter = new Vector2Int(4, 4),
                    PlacementCells = new List<Vector2Int>(placementItems_3),
                    Shape = grid_3,
                    RotationIndex = 0
                };

                _spawnData.PlacementItems.Add(first);
                _spawnData.PlacementItems.Add(second);
                _spawnData.PlacementItems.Add(third);

                inventorySystem.InventorySystem.TryRestorePlacement(first);
                inventorySystem.InventorySystem.TryRestorePlacement(second);
                inventorySystem.InventorySystem.TryRestorePlacement(third);
            }
        }
    }
}