namespace GridSystem.Editor.Validation
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Inspector drawing the default fields followed by the configuration defects of the asset.
    /// Validation runs when the inspector opens and after every edit, never on repaint.
    /// Derive and implement <see cref="Validate"/> to validate a custom configuration asset.
    /// </summary>
    public abstract class GridValidatedEditor : UnityEditor.Editor
    {
        private const string HEADER = "Validation";
        private const string VALID = "No configuration defects found.";

        private readonly GridValidationReport _report = new();

        /// <summary>
        /// Draws the asset fields and the validation summary.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck())
            {
                Revalidate();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(HEADER, EditorStyles.boldLabel);
            if (_report.IsEmpty)
            {
                EditorGUILayout.HelpBox(VALID, MessageType.Info);
            }
            else
            {
                DrawIssues();
            }
        }

        /// <summary>
        /// Collects the defects of the inspected asset into the report.
        /// </summary>
        protected abstract void Validate(GridValidationReport report);

        /// <summary>
        /// Validates the asset when the inspector opens.
        /// </summary>
        protected virtual void OnEnable() => Revalidate();

        private void Revalidate()
        {
            _report.Clear();
            Validate(_report);
        }

        private void DrawIssues()
        {
            for (int i = 0; i < _report.Issues.Count; i++)
            {
                GridValidationIssue issue = _report.Issues[i];
                MessageType type = issue.Severity == GridValidationSeverity.Error ? MessageType.Error : MessageType.Warning;
                EditorGUILayout.HelpBox(issue.Message, type);
                Rect rect = GUILayoutUtility.GetLastRect();
                if (issue.Context != null)
                {
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
                    if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                    {
                        EditorGUIUtility.PingObject(issue.Context);
                        Event.current.Use();
                    }
                }
            }
        }
    }
}
