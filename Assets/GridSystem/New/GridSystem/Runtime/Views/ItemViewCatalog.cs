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
        private const string WITHOUT_VIEW = "Catalog '{0}' has no view for '{1}'. Either add the entry or keep the item out of inventories shown by surfaces using this catalog.";
        private const string WITHOUT_PREFAB = "Catalog '{0}' has an entry for '{1}' without a view prefab.";

        [SerializeField]
        private ItemViewEntry[] _entries = Array.Empty<ItemViewEntry>();

        private readonly Dictionary<IGridItemDefinition, ItemView> _lookup = new();
        private bool _dirty = true;

        /// <summary>
        /// Returns view prefab of the definition. Throws when the catalog cannot show it: ask
        /// <see cref="Contains"/> first, a surface refuses what it cannot display.
        /// </summary>
        public ItemView GetPrefab(IGridItemDefinition definition)
        {
            EnsureBuilt();
            if (!_lookup.TryGetValue(definition, out ItemView prefab))
            {
                throw new KeyNotFoundException(string.Format(WITHOUT_VIEW, name, definition));
            }

            if (prefab == null)
            {
                throw new InvalidOperationException(string.Format(WITHOUT_PREFAB, name, definition));
            }

            return prefab;
        }

        /// <summary>
        /// Returns true when the definition has a view prefab.
        /// </summary>
        public bool Contains(IGridItemDefinition definition)
        {
            EnsureBuilt();
            return _lookup.ContainsKey(definition);
        }

        private void OnEnable() => _dirty = true;

#if UNITY_EDITOR
        private void OnValidate() => _dirty = true;
#endif

        private void EnsureBuilt()
        {
            if (!_dirty)
            {
                return;
            }

            _dirty = false;
            BuildLookup();
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
