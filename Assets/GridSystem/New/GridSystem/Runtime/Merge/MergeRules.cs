namespace GridSystem.Runtime.Merge
{
    using System;
    using System.Collections.Generic;
    using Core.Items;
    using Definitions;
    using UnityEngine;

    /// <summary>
    /// Pairs of definitions producing a new definition when one item is dropped onto another.
    /// </summary>
    [CreateAssetMenu(menuName = MENU_NAME, fileName = FILE_NAME)]
    public sealed class MergeRules : ScriptableObject, IMergeRules
    {
        private const string MENU_NAME = "Grid System/Merge Rules";
        private const string FILE_NAME = "MergeRules";

        [SerializeField]
        private MergeRule[] _rules = Array.Empty<MergeRule>();

        private readonly Dictionary<(IGridItemDefinition, IGridItemDefinition), IGridItemDefinition> _lookup = new();
        private bool _isBuilt;

        /// <summary>
        /// Finds merge result for the pair in any order.
        /// </summary>
        public bool TryGetResult(IGridItemDefinition first, IGridItemDefinition second, out IGridItemDefinition result)
            => GetLookup().TryGetValue((first, second), out result);

        private void OnEnable() => _isBuilt = false;

#if UNITY_EDITOR
        private void OnValidate() => _isBuilt = false;
#endif

        private Dictionary<(IGridItemDefinition, IGridItemDefinition), IGridItemDefinition> GetLookup()
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
            for (int i = 0; i < _rules.Length; i++)
            {
                _lookup[(_rules[i].First, _rules[i].Second)] = _rules[i].Result;
                _lookup[(_rules[i].Second, _rules[i].First)] = _rules[i].Result;
            }
        }
    }

    /// <summary>
    /// Single merge recipe.
    /// </summary>
    [Serializable]
    public struct MergeRule
    {
        /// <summary>
        /// First ingredient.
        /// </summary>
        public ItemDefinition First;

        /// <summary>
        /// Second ingredient.
        /// </summary>
        public ItemDefinition Second;

        /// <summary>
        /// Produced item.
        /// </summary>
        public ItemDefinition Result;
    }
}
