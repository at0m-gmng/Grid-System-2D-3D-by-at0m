namespace GridSystem.Runtime.Definitions
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// All item definitions known by the game. Resolves definitions by persistent id.
    /// </summary>
    [CreateAssetMenu(menuName = MENU_NAME, fileName = FILE_NAME)]
    public sealed class ItemDefinitionCatalog : ScriptableObject
    {
        private const string MENU_NAME = "Grid System/Item Definition Catalog";
        private const string FILE_NAME = "ItemDefinitionCatalog";

        [SerializeField]
        private ItemDefinition[] _items = Array.Empty<ItemDefinition>();

        private readonly Dictionary<string, ItemDefinition> _lookup = new();

        /// <summary>
        /// Registered definitions.
        /// </summary>
        public IReadOnlyList<ItemDefinition> Items => _items;

        /// <summary>
        /// Finds definition by persistent id.
        /// </summary>
        public bool TryGet(string id, out ItemDefinition definition) => _lookup.TryGetValue(id, out definition);

        private void OnEnable() => BuildLookup();

#if UNITY_EDITOR
        private void OnValidate() => BuildLookup();
#endif

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
