namespace GridSystem.Runtime.Views.Highlight
{
    using UnityEngine;

    /// <summary>
    /// Shows placement feedback on the item view.
    /// </summary>
    public abstract class ItemHighlighter : MonoBehaviour
    {
        [SerializeField]
        protected PlacementPalette palette = PlacementPalette.Default;

        /// <summary>
        /// Applies state visuals.
        /// </summary>
        public abstract void SetState(PlacementState state);
    }
}
