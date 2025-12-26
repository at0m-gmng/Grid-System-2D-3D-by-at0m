namespace GridSystem.EditorGridDrawled
{
    using System;
    using System.Collections.Generic;
    using Core;
    using UnityEngine;
    using Grid = Core.Grid;

    [Serializable]
    public sealed class EditorGridItem
    {
        public Grid Grid => _grid;

        [SerializeField]
        private int _rows = 4;

        [SerializeField]
        private int _columns = 4;

        [Range(1, 10)]
        [SerializeField]
        private int _cellSize = 1;

        [SerializeField]
        private Grid _grid;

        public void ResetGrid()
        {
            var mg = new Grid { Rows = new List<ColumnList>() };
            int targetRows = Mathf.Max(1, _rows);
            int targetColumns = Mathf.Max(1, _columns);

            for (int i = 0; i < targetRows; i++)
            {
                var row = new ColumnList { Columns = new List<int>() };
                for (int j = 0; j < targetColumns; j++)
                    row.Columns.Add(0);
                mg.Rows.Add(row);
            }

            _grid = mg;
        }

        public Grid GetGrid() => _grid;
    }
}