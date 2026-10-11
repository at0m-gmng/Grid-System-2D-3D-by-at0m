namespace GridSystem.Core.Model
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Flat occupancy matrix. Every cell stores the owner item id or <see cref="EMPTY"/>.
    /// </summary>
    public sealed class GridModel
    {
        public GridModel(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new int[width * height];
        }

        /// <summary>
        /// Value of a free cell.
        /// </summary>
        public const int EMPTY = 0;

        private readonly int[] _cells;

        /// <summary>
        /// Columns count.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Rows count.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Returns true when the cell is inside the grid.
        /// </summary>
        public bool Contains(Vector2Int cell) => (uint)cell.x < (uint)Width && (uint)cell.y < (uint)Height;

        /// <summary>
        /// Returns owner id of the cell inside the grid.
        /// </summary>
        public int GetOwner(Vector2Int cell) => _cells[cell.y * Width + cell.x];

        /// <summary>
        /// Checks that every shape cell is inside the grid and free or owned by ignored ids.
        /// </summary>
        public bool CanPlace(ReadOnlySpan<Vector2Int> cells, Vector2Int origin, int ignoredA, int ignoredB)
        {
            bool result = true;
            for (int i = 0; result && i < cells.Length; i++)
            {
                Vector2Int cell = origin + cells[i];
                result = Contains(cell) && IsAvailable(GetOwner(cell), ignoredA, ignoredB);
            }

            return result;
        }

        /// <summary>
        /// Writes owner id into shape cells without validation.
        /// </summary>
        public void Fill(ReadOnlySpan<Vector2Int> cells, Vector2Int origin, int owner)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Vector2Int cell = origin + cells[i];
                _cells[cell.y * Width + cell.x] = owner;
            }
        }

        /// <summary>
        /// Frees every cell.
        /// </summary>
        public void Clear() => Array.Clear(_cells, 0, _cells.Length);

        private static bool IsAvailable(int owner, int ignoredA, int ignoredB) => owner == EMPTY || owner == ignoredA || owner == ignoredB;
    }
}
