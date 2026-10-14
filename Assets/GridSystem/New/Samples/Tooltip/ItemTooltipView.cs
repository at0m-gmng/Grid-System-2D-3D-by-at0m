namespace GridSystem.Samples.Tooltip
{
    using Definitions;
    using TMPro;
    using UnityEngine;

    /// <summary>
    /// Passive tooltip widget.
    /// </summary>
    public sealed class ItemTooltipView : MonoBehaviour
    {
        private const string VALUE_FORMAT = "{0:0}";

        [SerializeField]
        private GameObject _root;

        [SerializeField]
        private RectTransform _panel;

        [SerializeField]
        private TMP_Text _name;

        [SerializeField]
        private TMP_Text _damage;

        /// <summary>
        /// Shows description near the screen point.
        /// </summary>
        public void Show(IItemDescription description, Vector2 screenPosition)
        {
            _name.SetText(description.DisplayName);
            _damage.SetText(VALUE_FORMAT, description.Damage);
            _panel.position = screenPosition;
            _root.SetActive(true);
        }

        /// <summary>
        /// Hides the tooltip.
        /// </summary>
        public void Hide() => _root.SetActive(false);
    }
}
