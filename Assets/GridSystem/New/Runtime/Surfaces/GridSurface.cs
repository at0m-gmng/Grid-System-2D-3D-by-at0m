namespace GridSystem.Runtime.Surfaces
{
    using System.Collections.Generic;
    using UnityEngine;
    using Views;

    /// <summary>
    /// Visual projection of a grid. Converts screen points, cells and world positions.
    /// Grid space: origin in top-left corner, x to the right, y down, in surface units.
    /// </summary>
    public abstract class GridSurface : MonoBehaviour
    {
        private const float HALF = 0.5f;

        [SerializeField]
        private ItemViewCatalog _views;

        [SerializeField]
        private Transform _itemsRoot;

        [SerializeField]
        private Transform _cellsRoot;

        [SerializeField]
        private Transform _cellPrefab;

        [SerializeField]
        private Vector2 _cellSize = Vector2.one;

        [SerializeField]
        private Vector2 _spacing;

        [Tooltip("Surfaces with higher priority are picked first when they overlap.")]
        [SerializeField]
        private int _priority;

        private readonly List<Transform> _cells = new();

        /// <summary>
        /// Item view prefabs used by this surface.
        /// </summary>
        public ItemViewCatalog Views => _views;

        /// <summary>
        /// Parent of item views.
        /// </summary>
        public Transform ItemsRoot => _itemsRoot;

        /// <summary>
        /// Cell size in surface units.
        /// </summary>
        public Vector2 CellSize => _cellSize;

        /// <summary>
        /// Gap between cells in surface units.
        /// </summary>
        public Vector2 Spacing => _spacing;

        /// <summary>
        /// Distance between neighbour cell origins.
        /// </summary>
        public Vector2 Pitch => _cellSize + _spacing;

        /// <summary>
        /// Picking order for overlapping surfaces.
        /// </summary>
        public int Priority => _priority;

        /// <summary>
        /// Built columns count.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Built rows count.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Total size of the built grid in surface units.
        /// </summary>
        public Vector2 Size => new Vector2(Width * Pitch.x - _spacing.x, Height * Pitch.y - _spacing.y);

        /// <summary>
        /// Builds cell visuals reusing existing instances.
        /// </summary>
        public void Build(int width, int height)
        {
            Width = width;
            Height = height;
            OnBuild();
            int count = width * height;
            for (int i = _cells.Count; i < count; i++)
            {
                _cells.Add(Instantiate(_cellPrefab, _cellsRoot));
            }

            for (int i = 0; i < _cells.Count; i++)
            {
                bool isUsed = i < count;
                _cells[i].gameObject.SetActive(isUsed);
                if (isUsed)
                {
                    PlaceCell(_cells[i], new Vector2Int(i % width, i / width));
                }
            }
        }

        /// <summary>
        /// Converts screen point to a cell inside the grid.
        /// </summary>
        public bool TryGetCell(Vector2 screenPosition, out Vector2Int cell)
        {
            cell = default;
            bool result = TryGetGridPoint(screenPosition, out Vector2 gridPoint);
            if (result)
            {
                cell = new Vector2Int(Mathf.FloorToInt(gridPoint.x / Pitch.x), Mathf.FloorToInt(gridPoint.y / Pitch.y));
                result = (uint)cell.x < (uint)Width && (uint)cell.y < (uint)Height;
            }

            return result;
        }

        /// <summary>
        /// Projects screen point onto the surface plane.
        /// </summary>
        public bool TryGetWorldPoint(Vector2 screenPosition, out Vector3 worldPosition)
        {
            worldPosition = default;
            bool result = TryGetGridPoint(screenPosition, out Vector2 gridPoint);
            if (result)
            {
                worldPosition = GridToWorld(gridPoint);
            }

            return result;
        }

        /// <summary>
        /// World position of the cell center.
        /// </summary>
        public Vector3 GetCellPosition(Vector2Int cell) => GridToWorld(new Vector2(cell.x * Pitch.x + _cellSize.x * HALF, cell.y * Pitch.y + _cellSize.y * HALF));

        /// <summary>
        /// World pose of an item pivot placed into the cell.
        /// </summary>
        public Pose GetItemPose(Vector2Int cell, int rotation) => new Pose(GetCellPosition(cell), GetRotation(rotation));

        /// <summary>
        /// World rotation for clockwise quarter turns.
        /// </summary>
        public abstract Quaternion GetRotation(int rotation);

        /// <summary>
        /// Converts screen point to grid space. Point may be outside of the grid.
        /// </summary>
        protected abstract bool TryGetGridPoint(Vector2 screenPosition, out Vector2 gridPoint);

        /// <summary>
        /// Converts grid space point to world position.
        /// </summary>
        protected abstract Vector3 GridToWorld(Vector2 gridPoint);

        /// <summary>
        /// Called before cells placement when grid size changes.
        /// </summary>
        protected virtual void OnBuild()
        {
        }

        /// <summary>
        /// Positions a cell visual.
        /// </summary>
        protected virtual void PlaceCell(Transform cell, Vector2Int coordinates)
            => cell.SetPositionAndRotation(GetCellPosition(coordinates), GetRotation(0));
    }
}
