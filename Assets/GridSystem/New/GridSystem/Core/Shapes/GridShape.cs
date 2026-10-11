namespace GridSystem.Core.Shapes
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Immutable runtime shape with precomputed cell offsets for every rotation.
    /// Offsets are relative to the pivot cell, x is column, y is row (down).
    /// </summary>
    public sealed class GridShape
    {
        private GridShape(int width, int height, Vector2Int pivot, Vector2Int[][] rotations)
        {
            Width = width;
            Height = height;
            Pivot = pivot;
            _rotations = rotations;
        }

        /// <summary>
        /// Amount of supported quarter turns.
        /// </summary>
        public const int ROTATION_COUNT = 4;

        private const int ROTATION_MASK = ROTATION_COUNT - 1;

        private readonly Vector2Int[][] _rotations;

        /// <summary>
        /// Columns count of the bounding rectangle without rotation.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Rows count of the bounding rectangle without rotation.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Pivot cell inside the bounding rectangle without rotation.
        /// </summary>
        public Vector2Int Pivot { get; }

        /// <summary>
        /// Converts any rotation value to range [0, 3].
        /// </summary>
        public static int NormalizeRotation(int rotation) => rotation & ROTATION_MASK;

        /// <summary>
        /// Returns pivot-relative cell offsets for the clockwise rotation.
        /// </summary>
        public ReadOnlySpan<Vector2Int> GetCells(int rotation) => _rotations[NormalizeRotation(rotation)];

        /// <summary>
        /// Builds runtime shape from serialized data.
        /// </summary>
        public static GridShape Create(in GridShapeData data)
        {
            Vector2Int[][] rotations = new Vector2Int[ROTATION_COUNT][];
            rotations[0] = CollectCells(data);
            for (int i = 1; i < ROTATION_COUNT; i++)
            {
                rotations[i] = RotateClockwise(rotations[i - 1]);
            }

            return new GridShape(data.Width, data.Height, data.Pivot, rotations);
        }

        private static Vector2Int[] CollectCells(in GridShapeData data)
        {
            int count = 0;
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    if (data.IsFilled(x, y))
                    {
                        count++;
                    }
                }
            }

            Vector2Int[] cells = new Vector2Int[count];
            int index = 0;
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    if (data.IsFilled(x, y))
                    {
                        cells[index] = new Vector2Int(x - data.Pivot.x, y - data.Pivot.y);
                        index++;
                    }
                }
            }

            return cells;
        }

        private static Vector2Int[] RotateClockwise(Vector2Int[] source)
        {
            Vector2Int[] rotated = new Vector2Int[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                rotated[i] = new Vector2Int(-source[i].y, source[i].x);
            }

            return rotated;
        }
    }
}
