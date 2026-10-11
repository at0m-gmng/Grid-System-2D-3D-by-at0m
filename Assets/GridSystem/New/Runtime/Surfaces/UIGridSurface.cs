namespace GridSystem.Runtime.Surfaces
{
    using UnityEngine;

    /// <summary>
    /// Canvas surface. Grid area is the rect of the target transform, sized on build.
    /// </summary>
    public sealed class UIGridSurface : GridSurface
    {
        private const float QUARTER_TURN = -90f;

        [SerializeField]
        private RectTransform _rect;

        [Tooltip("Canvas camera. Leave empty for Screen Space Overlay canvas.")]
        [SerializeField]
        private Camera _eventCamera;

        /// <summary>
        /// World rotation for clockwise quarter turns.
        /// </summary>
        public override Quaternion GetRotation(int rotation) => _rect.rotation * Quaternion.Euler(0f, 0f, QUARTER_TURN * rotation);

        protected override bool TryGetGridPoint(Vector2 screenPosition, out Vector2 gridPoint)
        {
            bool result = RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, screenPosition, _eventCamera, out Vector2 local);
            Rect rect = _rect.rect;
            gridPoint = new Vector2(local.x - rect.xMin, rect.yMax - local.y);
            return result;
        }

        protected override Vector3 GridToWorld(Vector2 gridPoint)
        {
            Rect rect = _rect.rect;
            return _rect.TransformPoint(new Vector3(rect.xMin + gridPoint.x, rect.yMax - gridPoint.y, 0f));
        }

        protected override void OnBuild()
        {
            Vector2 size = Size;
            _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }

        protected override void PlaceCell(Transform cell, Vector2Int coordinates)
        {
            ((RectTransform)cell).sizeDelta = CellSize;
            base.PlaceCell(cell, coordinates);
        }
    }
}
