namespace GridSystem.EditorGridDrawled.Examples
{
    using UnityEditor;
    using Grid = Core.Grid;
#if UNITY_EDITOR
    using UnityEngine;
#endif

    [CreateAssetMenu(fileName = "ExampleEditorGridItem", menuName = "Example/Configs/ExampleEditorGridItem")]
    public sealed class ExampleEditorGridItem : ScriptableObject
    {
        [SerializeField]
        private Grid _grid;

        [SerializeField]
        private EditorGridItem _editorGrid = new EditorGridItem();

#if UNITY_EDITOR
        private void OnValidate()
        {
            EditorUtility.SetDirty(this);
            _grid = _editorGrid.GetGrid();
        }
#endif
    }
}