namespace GridSystem.Core.Shapes
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Serializable rectangular shape description with filled cells and pivot cell.
    /// Cells are stored row-major, row 0 is the top row.
    /// </summary>
    [Serializable]
    public struct GridShapeData
    {
        public GridShapeData(int width, int height, bool[] cells, Vector2Int pivot)
        {
            _width = width;
            _height = height;
            _cells = cells;
            _pivot = pivot;
        }

        [SerializeField]
        private int _width;

        [SerializeField]
        private int _height;

        [SerializeField]
        private bool[] _cells;

        [SerializeField]
        private Vector2Int _pivot;

        /// <summary>
        /// Columns count of the bounding rectangle.
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// Rows count of the bounding rectangle.
        /// </summary>
        public int Height => _height;

        /// <summary>
        /// Cell used as placement anchor and rotation center.
        /// </summary>
        public Vector2Int Pivot => _pivot;

        /// <summary>
        /// Returns true when the cell belongs to the shape.
        /// </summary>
        public bool IsFilled(int x, int y) => _cells[y * _width + x];
    }
}
