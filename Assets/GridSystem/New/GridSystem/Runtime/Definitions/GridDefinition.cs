namespace GridSystem.Runtime.Definitions
{
    using UnityEngine;

    /// <summary>
    /// Inventory grid size.
    /// </summary>
    [CreateAssetMenu(menuName = MENU_NAME, fileName = FILE_NAME)]
    public sealed class GridDefinition : ScriptableObject
    {
        private const string MENU_NAME = "Grid System/Grid Definition";
        private const string FILE_NAME = "GridDefinition";

        [SerializeField, Min(1)]
        private int _width = 1;

        [SerializeField, Min(1)]
        private int _height = 1;

        /// <summary>
        /// Columns count.
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// Rows count.
        /// </summary>
        public int Height => _height;
    }
}
