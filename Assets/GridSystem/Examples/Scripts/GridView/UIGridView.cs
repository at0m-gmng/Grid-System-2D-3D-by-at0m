namespace GridSystem.GridView
{
    using System.Collections.Generic;
    using InventoryPresenters;
    using InventorySystem.Inventory.Core;
    using Signals;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public sealed class UIGridView : AbstractGridView<RectTransform, UIInventoryPresenter>
    {
        protected override void Construct(SignalBus signalBus)
        {
            base.Construct(signalBus);
            this.signalBus.Subscribe<CreatedItemSignal>(InitializePlacement);
        }

        [SerializeField]
        private GridLayoutGroup _gridLayout;

        protected override void Initialize(int rows, int columns)
        {
            UpdateGrid(rows, columns);
            itemParent.SetAsLastSibling();
            if (transform.parent.childCount > 2)
            {
                outsideParent.SetSiblingIndex(transform.parent.childCount - 2);
            }
            else
            {
                outsideParent.SetAsLastSibling();
            }
        }

        protected override UIInventoryPresenter CreatePresenter(InventorySystemData inventorySystem)
            => new UIInventoryPresenter(_gridLayout, itemParent, cellObjects, inventorySystem.InventorySystem);

        protected override void UpdateGrid(int rows, int cols)
        {
            BuildGrid(rows, cols);
            UpdateField(rows, cols);
        }

        private void BuildGrid(int rows, int cols)
        {
            bool isFixRows = rows <= cols;
            _gridLayout.constraint = isFixRows ? GridLayoutGroup.Constraint.FixedRowCount : GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = isFixRows ? rows : cols;
        }

        private void UpdateField(int rows, int cols)
        {
            for (int i = cellObjects.Count; i < rows; i++)
            {
                cellObjects.Add(new List<RectTransform>());
            }

            if (rows < cellObjects.Count)
            {
                for (int i = cellObjects.Count - 1; i >= rows; i--)
                {
                    foreach (var cell in cellObjects[i])
                    {
                        if (cell != null)
                        {
                            Destroy(cell.gameObject);
                        }
                    }

                    cellObjects.RemoveAt(i);
                }
            }

            for (int i = 0; i < cellObjects.Count; i++)
            {
                for (int j = cellObjects[i].Count; j < cols; j++)
                {
                    RectTransform gridCell = Instantiate(cellPrefab, _gridLayout.transform);
                    gridCell.gameObject.name = $"{cellPrefab.name}_{i}_{j}";
                    cellObjects[i].Add(gridCell);
                }

                if (cols < cellObjects[i].Count)
                {
                    for (int j = cellObjects[i].Count - 1; j >= cols; j--)
                    {
                        if (cellObjects[i][j] != null)
                        {
                            Destroy(cellObjects[i][j].gameObject);
                        }

                        cellObjects[i].RemoveAt(j);
                    }
                }
            }
        }

        private void InitializePlacement(CreatedItemSignal signal)
        {
            if (signal.UIDraggableObject.InventoryView == InventoryPresenter)
            {
                InventoryPresenter.TryAutoPlaceItem(signal.UIDraggableObject);
            }
        }
    }
}