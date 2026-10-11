namespace GridSystem.Runtime.Views
{
    using Core.Items;
    using Highlight;
    using Motion;
    using Surfaces;
    using UnityEngine;

    /// <summary>
    /// Pooled passive item visual. Transform origin is the center of the pivot cell.
    /// </summary>
    public abstract class ItemView : MonoBehaviour
    {
        [SerializeField]
        private ItemMotion _motion;

        [SerializeField]
        private ItemHighlighter _highlighter;

        [SerializeField]
        private GameObject _body;

        /// <summary>
        /// Shown item definition.
        /// </summary>
        public IGridItemDefinition Definition { get; private set; }

        internal ItemView Prefab { get; set; }

        /// <summary>
        /// Current world pose.
        /// </summary>
        public Pose Pose
        {
            get
            {
                transform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
                return new Pose(position, rotation);
            }
        }

        /// <summary>
        /// Prepares the view for the definition on the surface.
        /// </summary>
        public void Setup(IGridItemDefinition definition, GridSurface surface)
        {
            Definition = definition;
            OnSetup(definition, surface);
            SetPlacementState(PlacementState.Normal);
            SetVisible(true);
        }

        /// <summary>
        /// Sets pose immediately.
        /// </summary>
        public void Snap(Pose pose) => _motion.Snap(pose);

        /// <summary>
        /// Animates to the pose.
        /// </summary>
        public void MoveTo(Pose pose) => _motion.MoveTo(pose);

        /// <summary>
        /// Sets position immediately keeping rotation animation.
        /// </summary>
        public void Follow(Vector3 position) => _motion.Follow(position);

        /// <summary>
        /// Animates rotation.
        /// </summary>
        public void RotateTo(Quaternion rotation) => _motion.RotateTo(rotation);

        /// <summary>
        /// Applies drag feedback.
        /// </summary>
        public void SetPlacementState(PlacementState state) => _highlighter.SetState(state);

        /// <summary>
        /// Shows or hides the body keeping the view registered.
        /// </summary>
        public void SetVisible(bool isVisible) => _body.SetActive(isVisible);

        internal void Release()
        {
            _motion.Stop();
            Definition = null;
        }

        /// <summary>
        /// Adapts view geometry to the shape and the surface cell size.
        /// </summary>
        protected abstract void OnSetup(IGridItemDefinition definition, GridSurface surface);
    }
}
