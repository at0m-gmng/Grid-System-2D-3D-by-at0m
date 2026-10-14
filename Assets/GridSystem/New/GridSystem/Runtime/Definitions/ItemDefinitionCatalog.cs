namespace GridSystem.Runtime.Definitions
{
    using System;
    using System.Collections.Generic;
    using Core.Items;
    using UnityEngine;

    /// <summary>
    /// All item definitions known by the game. Resolves definitions by persistent id.
    /// </summary>
    [CreateAssetMenu(menuName = MENU_NAME, fileName = FILE_NAME)]
    public sealed class ItemDefinitionCatalog : ScriptableObject, IItemDefinitionCatalog
    {
        private const string MENU_NAME = "Grid System/Item Definition Catalog";
        private const string FILE_NAME = "ItemDefinitionCatalog";

        [SerializeField]
        private ItemDefinition[] _items = Array.Empty<ItemDefinition>();

        private readonly Dictionary<string, IGridItemDefinition> _lookup = new();
        private bool _dirty = true;

        /// <summary>
        /// Finds definition by persistent id.
        /// </summary>
        public bool TryGet(string id, out IGridItemDefinition definition)
        {
            EnsureBuilt();
            return _lookup.TryGetValue(id, out definition);
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
            for (int i = 0; i < _items.Length; i++)
            {
                _lookup[_items[i].Id] = _items[i];
            }
        }
    }
}
