#if DOTWEEN
namespace GridSystem.Runtime.Views.Motion
{
    using DG.Tweening;
    using UnityEngine;

    /// <summary>
    /// Motion based on DOTween. Requires DOTWEEN define. Enable tween recycling to avoid allocations.
    /// </summary>
    public sealed class DOTweenItemMotion : ItemMotion
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
            _position = target.DOMove(pose.position, _moveDuration).SetEase(_ease);
            _rotation = target.DORotateQuaternion(pose.rotation, _rotateDuration).SetEase(_ease);
        }

        /// <summary>
        /// Stops position animation and sets position immediately.
        /// </summary>
        public override void Follow(Vector3 position)
        {
            Kill(_position);
            target.position = position;
        }

        /// <summary>
        /// Animates rotation.
        /// </summary>
        public override void RotateTo(Quaternion rotation)
        {
            Kill(_rotation);
            _rotation = target.DORotateQuaternion(rotation, _rotateDuration).SetEase(_ease);
        }

        /// <summary>
        /// Stops all animations.
        /// </summary>
        public override void Stop()
        {
            Kill(_position);
            Kill(_rotation);
        }

        private static void Kill(Tween tween)
        {
            if (tween.IsActive())
            {
                tween.Kill();
            }
        }
    }
}
#endif
