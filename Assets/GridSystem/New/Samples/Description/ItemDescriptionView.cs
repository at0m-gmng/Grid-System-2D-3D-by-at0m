namespace GridSystem.Samples.Description
{
    using Definitions;
    using TMPro;
    using UnityEngine;

    /// <summary>
    /// Passive description popup of the hovered item.
    /// </summary>
    public sealed class ItemDescriptionView : MonoBehaviour
    {
        private const string VALUE_FORMAT = "{0:2}";

        [SerializeField]
        private GameObject _root;

        [SerializeField]
        private TMP_Text _name;

        [SerializeField]
        private TMP_Text _damage;

        [SerializeField]
        private TMP_Text _attackSpeed;

        [SerializeField]
        private TMP_Text _range;

        /// <summary>
        /// Shows item values. Texts are formatted without allocations.
        /// </summary>
        public void Show(IItemDescription description)
        {
            _name.SetText(description.DisplayName);
            _damage.SetText(VALUE_FORMAT, description.Damage);
            _attackSpeed.SetText(VALUE_FORMAT, description.AttackSpeed);
            _range.SetText(VALUE_FORMAT, description.Range);
            _root.SetActive(true);
        }

        /// <summary>
        /// Hides the popup.
        /// </summary>
        public void Hide() => _root.SetActive(false);
    }
}
