namespace GridSystem.Samples.Definitions
{
    using Runtime.Definitions;
    using UnityEngine;

    /// <summary>
    /// Sample item with weapon stats. Shows how to extend <see cref="ItemDefinition"/>.
    /// </summary>
    [CreateAssetMenu(menuName = MENU_NAME, fileName = FILE_NAME)]
    public sealed class SampleItemDefinition : ItemDefinition, IItemDescription
    {
        private const string MENU_NAME = "Grid System/Samples/Sample Item Definition";
        private const string FILE_NAME = "SampleItemDefinition";

        [SerializeField]
        private string _displayName;

        [SerializeField]
        private float _damage;

        [SerializeField]
        private float _attackSpeed;

        [SerializeField]
        private float _range;

        /// <summary>
        /// Display name.
        /// </summary>
        public string DisplayName => _displayName;

        /// <summary>
        /// Damage value.
        /// </summary>
        public float Damage => _damage;

        /// <summary>
        /// Attack speed value.
        /// </summary>
        public float AttackSpeed => _attackSpeed;

        /// <summary>
        /// Range value.
        /// </summary>
        public float Range => _range;
    }
}
