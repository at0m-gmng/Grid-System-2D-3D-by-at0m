namespace GridSystem.Runtime.Views
{
    using Core.Items;
    using Core.Shapes;
    using Surfaces;
    using UnityEngine;

    /// <summary>
    /// 3D item view. Model must be centered on its bounds and sized in surface units;
    /// it is shifted so the transform origin matches the pivot cell center.
    /// </summary>
    public sealed class WorldItemView : ItemView
    {
        private const float HALF = 0.5f;

        [SerializeField]
        private Transform _model;

        protected override void OnSetup(IGridItemDefinition definition, GridSurface surface)
        {
            GridShape shape = definition.Shape;
            Vector2 pitch = surface.Pitch;
            float offsetX = ((shape.Width - 1) * HALF - shape.Pivot.x) * pitch.x;
            float offsetZ = (shape.Pivot.y - (shape.Height - 1) * HALF) * pitch.y;
            _model.localPosition = new Vector3(offsetX, 0f, offsetZ);
        }
    }
}
