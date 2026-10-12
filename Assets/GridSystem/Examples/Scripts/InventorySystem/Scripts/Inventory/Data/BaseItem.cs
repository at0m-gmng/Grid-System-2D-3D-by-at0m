namespace GridSystem.InventorySystem.Inventory.Data
{
    using System;
    using System.Collections.Generic;
    using DragDropSystem.View;
    using EditorGridDrawled;
    using Enums;
    using GridSystem.Core;
    using UnityEngine;
    using Grid = GridSystem.Core.Grid;

    [Serializable]
    public struct BaseItem
    {
        public BaseItem(string id, ItemType type = ItemType.None, int level = default, UIDraggableObject uiPrefab = default,
            GameObject worldPrefab = default, EditorGridItem editorGrid = default, bool isRotatable = true, bool isMergable = true)
        {
            Id = id;
            Type = type;
            Level = level;
            UIPrefab = uiPrefab;
            WorldPrefab = worldPrefab;
            Grid = editorGrid != null ? Grid.Clone(editorGrid.GetGrid()) : new Grid { Rows = new List<ColumnList>() };
            EditorGrid = editorGrid != null ? editorGrid : new EditorGridItem();
            IsRotatable = isRotatable;
            IsMergable = isMergable;
        }

        [field: SerializeField] public Grid Grid { get; set; }

        [Header("Identity")]
        [field: SerializeField]
        public string Id { get; private set; }

        [field: SerializeField] public ItemType Type { get; private set; }
        [field: SerializeField] public int Level { get; private set; }

        [Header("Prefab for Canvas")]
        [field: SerializeField]
        public UIDraggableObject UIPrefab { get; private set; }

        [Header("Prefab for 3D World")]
        [field: SerializeField]
        public GameObject WorldPrefab { get; private set; }

        [Header("Options")]
        [field: SerializeField]
        public bool IsRotatable { get; private set; }

        [field: SerializeField] public bool IsMergable { get; private set; }

#if UNITY_EDITOR
        [Header("Editor Helpful Core")]
        [field: SerializeField]
        public EditorGridItem EditorGrid { get; private set; }
#endif

        public Grid TryGetItemSize() => Grid;

        public BaseItem GetRotation(int rotationCount)
        {
            rotationCount = ((rotationCount % 4) + 4) % 4;
            Grid position = Grid.Clone(Grid);
            for (int i = 0; i < rotationCount; i++)
            {
                position = Grid.Clone(RotateOnce(position));
            }

            BaseItem rotatedItem = this;
            rotatedItem.Grid = Grid.Clone(position);
            return rotatedItem;
        }

        private Grid RotateOnce(Grid shape)
        {
            if (shape.Rows != null && shape.Rows.Count != 0)
            {
                Grid result = new Grid { Rows = new List<ColumnList>(shape.Rows[0].Columns.Count) };

                for (int i = 0; i < shape.Rows[0].Columns.Count; i++)
                {
                    ColumnList newRow = new ColumnList { Columns = new List<int>(shape.Rows.Count) };
                    for (int j = 0; j < shape.Rows.Count; j++)
                    {
                        int value = shape.Rows[shape.Rows.Count - 1 - j].Columns[i];
                        newRow.Columns.Add(value);
                    }

                    result.Rows.Add(newRow);
                }

                return result;
            }

            return new Grid { Rows = new List<ColumnList>() };
        }
    }
}