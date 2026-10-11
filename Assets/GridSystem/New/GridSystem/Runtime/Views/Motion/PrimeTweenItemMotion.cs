#if GRID_SYSTEM_PRIMETWEEN
namespace GridSystem.Runtime.Views.Motion
{
    using PrimeTween;
    using UnityEngine;

    /// <summary>
    /// Allocation free motion based on PrimeTween. Requires GRID_SYSTEM_PRIMETWEEN define.
    /// </summary>
    public sealed class PrimeTweenItemMotion : ItemMotion
    {
        private const float DEFAULT_MOVE_DURATION = 0.15f;
        private const float DEFAULT_ROTATE_DURATION = 0.1f;

        [SerializeField, Min(0f)]
        private float _moveDuration = DEFAULT_MOVE_DURATION;

        [SerializeField, Min(0f)]
        private float _rotateDuration = DEFAULT_ROTATE_DURATION;

        [SerializeField]
        private Ease _ease = Ease.OutQuad;

        private Tween _position;
        private Tween _rotation;

        /// <summary>
        /// Stops animations and sets pose immediately.
        /// </summary>
        public override void Snap(Pose pose)
        {
            Stop();
            target.SetPositionAndRotation(pose.position, pose.rotation);
        }

        /// <summary>
        /// Animates position and rotation.
        /// </summary>
        public override void MoveTo(Pose pose)
        {
            Stop();
            _position = Tween.Position(target, pose.position, _moveDuration, _ease);
            _rotation = Tween.Rotation(target, pose.rotation, _rotateDuration, _ease);
        }

        /// <summary>
        /// Stops position animation and sets position immediately.
        /// </summary>
        public override void Follow(Vector3 position)
        {
            _position.Stop();
            target.position = position;
        }

        /// <summary>
        /// Animates rotation.
        /// </summary>
        public override void RotateTo(Quaternion rotation)
        {
            _rotation.Stop();
            _rotation = Tween.Rotation(target, rotation, _rotateDuration, _ease);
        }

        /// <summary>
        /// Stops all animations.
        /// </summary>
        public override void Stop()
        {
            _position.Stop();
            _rotation.Stop();
        }
    }
}
#endif
