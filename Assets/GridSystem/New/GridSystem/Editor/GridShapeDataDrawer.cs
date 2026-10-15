namespace GridSystem.Editor
{
    using Core.Shapes;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Inspector editor of item shape: size fields, display scale and clickable cells matrix.
    /// </summary>
    [CustomPropertyDrawer(typeof(GridShapeData))]
    public sealed class GridShapeDataDrawer : PropertyDrawer
    {
        private const string WIDTH = "_width";
        private const string HEIGHT = "_height";
        private const string CELLS = "_cells";
        private const string PIVOT = "_pivot";
        private const string WIDTH_LABEL = "Width";
        private const string HEIGHT_LABEL = "Height";
        private const string CELL_SIZE_LABEL = "Cell Size";
        private const string CENTER_LABEL = "Center";
        private const string BUSY_LABEL = "Busy";
        private const string EMPTY_LABEL = "Empty";
        private const string HELP = "LMB: toggle cell. RMB: set pivot.";
        private const string CELL_SIZE_KEY = "GridSystem.Editor.ShapeCellSize";
        private const float CELL_GAP = 2f;
        private const float SPACING = 2f;
        private const float HALF = 0.5f;
        private const float MIN_CELL_SIZE = 24f;
        private const float MAX_CELL_SIZE = 72f;
        private const float DEFAULT_CELL_SIZE = 46f;
        private const int MIN_SIZE = 1;
        private const int MAX_SIZE = 16;
        private const int RIGHT_BUTTON = 1;
        private const int LINES = 4;

        private static float CellSize => EditorPrefs.GetFloat(CELL_SIZE_KEY, DEFAULT_CELL_SIZE);

        /// <summary>
        /// Total drawer height.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int height = Mathf.Max(MIN_SIZE, property.FindPropertyRelative(HEIGHT).intValue);
            return (EditorGUIUtility.singleLineHeight + SPACING) * LINES + height * (CellSize + CELL_GAP);
        }

        /// <summary>
        /// Draws size fields, display scale, cells and hint.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty width = property.FindPropertyRelative(WIDTH);
            SerializedProperty height = property.FindPropertyRelative(HEIGHT);
            SerializedProperty cells = property.FindPropertyRelative(CELLS);
            SerializedProperty pivot = property.FindPropertyRelative(PIVOT);
            float line = EditorGUIUtility.singleLineHeight;

            Rect row = new Rect(position.x, position.y, position.width, line);
            EditorGUI.LabelField(row, label);

            row.y += line + SPACING;
            Rect widthRect = new Rect(row.x, row.y, row.width * HALF - SPACING, line);
            Rect heightRect = new Rect(row.x + row.width * HALF, row.y, row.width * HALF, line);
            int newWidth = Mathf.Clamp(EditorGUI.IntField(widthRect, WIDTH_LABEL, width.intValue), MIN_SIZE, MAX_SIZE);
            int newHeight = Mathf.Clamp(EditorGUI.IntField(heightRect, HEIGHT_LABEL, height.intValue), MIN_SIZE, MAX_SIZE);
            if (newWidth != width.intValue || newHeight != height.intValue || cells.arraySize != newWidth * newHeight)
            {
                Resize(width, height, cells, pivot, newWidth, newHeight);
            }

            row.y += line + SPACING;
            Rect sizeRect = new Rect(row.x, row.y, row.width, line);
            float newCellSize = EditorGUI.Slider(sizeRect, CELL_SIZE_LABEL, CellSize, MIN_CELL_SIZE, MAX_CELL_SIZE);
            if (!Mathf.Approximately(newCellSize, CellSize))
            {
                EditorPrefs.SetFloat(CELL_SIZE_KEY, newCellSize);
            }

            float cellSize = CellSize;
            float top = row.y + line + SPACING;
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    Rect cellRect = new Rect(position.x + x * (cellSize + CELL_GAP), top + y * (cellSize + CELL_GAP), cellSize, cellSize);
                    DrawCell(cellRect, cells.GetArrayElementAtIndex(y * newWidth + x), pivot, new Vector2Int(x, y));
                }
            }

            Rect helpRect = new Rect(position.x, top + newHeight * (cellSize + CELL_GAP), position.width, line);
            EditorGUI.LabelField(helpRect, HELP, EditorStyles.miniLabel);
            EditorGUI.EndProperty();
        }

        private static void DrawCell(Rect rect, SerializedProperty cell, SerializedProperty pivot, Vector2Int coordinates)
        {
            bool isPivot = pivot.vector2IntValue == coordinates;
            Event current = Event.current;
            if (current.type == EventType.MouseDown && current.button == RIGHT_BUTTON && rect.Contains(current.mousePosition))
            {
                pivot.vector2IntValue = coordinates;
                cell.boolValue = true;
                current.Use();
            }

            Color previous = GUI.color;
            GUI.color = isPivot ? Color.yellow : cell.boolValue ? Color.red : Color.green;
            string caption = isPivot ? CENTER_LABEL : cell.boolValue ? BUSY_LABEL : EMPTY_LABEL;
            if (GUI.Button(rect, caption) && !isPivot)
            {
                cell.boolValue = !cell.boolValue;
            }

            GUI.color = previous;
        }

        private static void Resize(SerializedProperty width, SerializedProperty height, SerializedProperty cells, SerializedProperty pivot,
            int newWidth, int newHeight)
        {
            int oldWidth = width.intValue;
            int oldHeight = height.intValue;
            bool[] oldCells = new bool[cells.arraySize];
            for (int i = 0; i < oldCells.Length; i++)
            {
                oldCells[i] = cells.GetArrayElementAtIndex(i).boolValue;
            }

            cells.arraySize = newWidth * newHeight;
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    int oldIndex = y * oldWidth + x;
                    bool isKept = x < oldWidth && y < oldHeight && oldIndex < oldCells.Length;
                    cells.GetArrayElementAtIndex(y * newWidth + x).boolValue = isKept && oldCells[oldIndex];
                }
            }

            Vector2Int clampedPivot = new Vector2Int(Mathf.Min(pivot.vector2IntValue.x, newWidth - 1), Mathf.Min(pivot.vector2IntValue.y, newHeight - 1));
            pivot.vector2IntValue = clampedPivot;
            cells.GetArrayElementAtIndex(clampedPivot.y * newWidth + clampedPivot.x).boolValue = true;
            width.intValue = newWidth;
            height.intValue = newHeight;
        }
    }
}
