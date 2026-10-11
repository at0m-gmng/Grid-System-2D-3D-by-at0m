namespace GridSystem.Runtime.Views.Highlight
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Tints a UI graphic.
    /// </summary>
    public sealed class GraphicItemHighlighter : ItemHighlighter
    {
        [SerializeField]
        private Graphic _graphic;

        /// <summary>
        /// Applies state color.
        /// </summary>
        public override void SetState(PlacementState state) => _graphic.color = palette.Get(state);
    }
}
