namespace GridSystem.Runtime.Views
{
    using Core.Items;
    using Core.Shapes;
    using Surfaces;
    using UnityEngine;

    /// <summary>
    /// Canvas item view. Rect is resized to the shape bounds, pivot moved to the pivot cell center.
    /// On bar surfaces the vertical pivot stays centered, so items of any height share one center line.
    /// </summary>
    public sealed class UIItemView : ItemView
    {
        private const float HALF = 0.5f;

        [SerializeField]
        private RectTransform _rect;

        protected override void OnSetup(IGridItemDefinition definition, GridSurface surface)
        {
            GridShape shape = definition.Shape;
            Vector2 pitch = surface.Pitch;
            Vector2 size = new Vector2(shape.Width * pitch.x - surface.Spacing.x, shape.Height * pitch.y - surface.Spacing.y);
            Vector2 pivotOffset = new Vector2(shape.Pivot.x * pitch.x + surface.CellSize.x * HALF, shape.Pivot.y * pitch.y + surface.CellSize.y * HALF);
            _rect.sizeDelta = size;
            _rect.pivot = new Vector2(pivotOffset.x / size.x, surface.IsBar ? HALF : 1f - pivotOffset.y / size.y);
            _rect.SetAsLastSibling();
        }
    }
}
