namespace GridSystem.Runtime.Views
{
    using System;
    using System.Collections.Generic;
    using Core.Items;
    using Definitions;
    using UnityEngine;

    /// <summary>
    /// Maps item definitions to view prefabs of one surface kind (UI, World, etc.).
    /// </summary>
    [CreateAssetMenu(menuName = MENU_NAME, fileName = FILE_NAME)]
    public sealed class ItemViewCatalog : ScriptableObject
    {
        private const string MENU_NAME = "Grid System/Item View Catalog";
        private const string FILE_NAME = "ItemViewCatalog";

        [SerializeField]
        private ItemViewEntry[] _entries = Array.Empty<ItemViewEntry>();

        private readonly Dictionary<IGridItemDefinition, ItemView> _lookup = new();
        private bool _isBuilt;

        /// <summary>
        /// Returns view prefab of the definition.
        /// </summary>
        public ItemView GetPrefab(IGridItemDefinition definition) => GetLookup()[definition];

        /// <summary>
        /// Returns true when the definition has a view prefab.
        /// </summary>
        public bool Contains(IGridItemDefinition definition) => GetLookup().ContainsKey(definition);

        private void OnEnable() => _isBuilt = false;

#if UNITY_EDITOR
        private void OnValidate() => _isBuilt = false;
#endif

        private Dictionary<IGridItemDefinition, ItemView> GetLookup()
        {
            if (!_isBuilt)
            {
                BuildLookup();
                _isBuilt = true;
            }

            return _lookup;
        }

        private void BuildLookup()
        {
            _lookup.Clear();
            for (int i = 0; i < _entries.Length; i++)
            {
                _lookup[_entries[i].Definition] = _entries[i].Prefab;
            }
        }
    }

    /// <summary>
    /// Definition to prefab pair.
    /// </summary>
    [Serializable]
    public struct ItemViewEntry
    {
        /// <summary>
        /// Item definition.
        /// </summary>
        public ItemDefinition Definition;

        /// <summary>
        /// View prefab.
        /// </summary>
        public ItemView Prefab;
    }
}
