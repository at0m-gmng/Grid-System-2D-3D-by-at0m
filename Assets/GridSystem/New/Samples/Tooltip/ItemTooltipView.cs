namespace GridSystem.Samples.Tooltip
{
    using Definitions;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Passive tooltip widget. Texts are formatted only when the hovered item changes.
    /// </summary>
    public sealed class ItemTooltipView : MonoBehaviour
    {
        private const string VALUE_FORMAT = "0.##";

        [SerializeField]
        private GameObject _root;

        [SerializeField]
        private RectTransform _panel;

        [SerializeField]
        private Text _name;

        [SerializeField]
        private Text _damage;

        [SerializeField]
        private Text _attackSpeed;

        [SerializeField]
        private Text _range;

        /// <summary>
        /// Shows description near the screen point.
        /// </summary>
        public void Show(IItemDescription description, Vector2 screenPosition)
        {
            _name.text = description.DisplayName;
            _damage.text = description.Damage.ToString(VALUE_FORMAT);
            _attackSpeed.text = description.AttackSpeed.ToString(VALUE_FORMAT);
            _range.text = description.Range.ToString(VALUE_FORMAT);
            _panel.position = screenPosition;
            _root.SetActive(true);
        }

        /// <summary>
        /// Hides the tooltip.
        /// </summary>
        public void Hide() => _root.SetActive(false);
    }
}
