namespace GridSystem.Runtime.Views.Motion
{
    using UnityEngine;

    /// <summary>
    /// Moves item view transform. Implementations choose the animation backend.
    /// </summary>
    public abstract class ItemMotion : MonoBehaviour
    {
        [SerializeField]
        protected Transform target;

        /// <summary>
        /// Stops animations and sets pose immediately.
        /// </summary>
        public abstract void Snap(Pose pose);

        /// <summary>
        /// Animates position and rotation.
        /// </summary>
        public abstract void MoveTo(Pose pose);

        /// <summary>
        /// Stops position animation and sets position immediately.
        /// </summary>
        public abstract void Follow(Vector3 position);

        /// <summary>
        /// Animates rotation.
        /// </summary>
        public abstract void RotateTo(Quaternion rotation);

        /// <summary>
        /// Stops all animations.
        /// </summary>
        public abstract void Stop();
    }
}
