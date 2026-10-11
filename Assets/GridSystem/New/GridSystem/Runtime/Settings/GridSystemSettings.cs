namespace GridSystem.Runtime.Settings
{
    using Definitions;
    using Merge;
    using Pointer;
    using UnityEngine;

    /// <summary>
    /// Root configuration of the grid system.
    /// </summary>
    [CreateAssetMenu(menuName = MENU_NAME, fileName = FILE_NAME)]
    public sealed class GridSystemSettings : ScriptableObject
    {
        private const string MENU_NAME = "Grid System/Settings";
        private const string FILE_NAME = "GridSystemSettings";

        [SerializeField]
        private ItemDefinitionCatalog _items;

        [SerializeField]
        private MergeRules _mergeRules;

        [SerializeField]
        private PointerInputConfig _input;

        /// <summary>
        /// Known item definitions.
        /// </summary>
        public ItemDefinitionCatalog Items => _items;

        /// <summary>
        /// Merge recipes.
        /// </summary>
        public MergeRules MergeRules => _mergeRules;

        /// <summary>
        /// Pointer input actions.
        /// </summary>
        public PointerInputConfig Input => _input;
    }
}
