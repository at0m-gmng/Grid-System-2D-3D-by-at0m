namespace GridSystem.Runtime.Definitions
{
    using Core.Items;
    using Core.Shapes;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// Item data asset. Inherit to add game specific data.
    /// </summary>
    [CreateAssetMenu(menuName = MENU_NAME, fileName = FILE_NAME)]
    public class ItemDefinition : ScriptableObject, IGridItemDefinition
    {
        private const string MENU_NAME = "Grid System/Item Definition";
        private const string FILE_NAME = "ItemDefinition";

        [SerializeField]
        private string _id;

        [SerializeField]
        private bool _isRotatable = true;

        [SerializeField]
        private GridShapeData _shape;

        /// <summary>
        /// Persistent unique identifier, generated from asset GUID.
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Runtime shape with precomputed rotations.
        /// </summary>
        public GridShape Shape { get; private set; }

        /// <summary>
        /// Allows rotation by the user and auto placement.
        /// </summary>
        public bool IsRotatable => _isRotatable;

        protected virtual void OnEnable() => Shape = GridShape.Create(_shape);

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            Shape = GridShape.Create(_shape);
            if (string.IsNullOrEmpty(_id))
            {
                _id = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}
