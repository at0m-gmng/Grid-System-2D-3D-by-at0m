namespace GridSystem.Runtime.Views.Motion
{
    using UnityEngine;

    /// <summary>
    /// Motion without animation.
    /// </summary>
    public sealed class InstantItemMotion : ItemMotion
    {
        /// <summary>
        /// Sets pose immediately.
        /// </summary>
        public override void Snap(Pose pose) => target.SetPositionAndRotation(pose.position, pose.rotation);

        /// <summary>
        /// Sets pose immediately.
        /// </summary>
        public override void MoveTo(Pose pose) => target.SetPositionAndRotation(pose.position, pose.rotation);

        /// <summary>
        /// Sets position immediately.
        /// </summary>
        public override void Follow(Vector3 position) => target.position = position;

        /// <summary>
        /// Sets rotation immediately.
        /// </summary>
        public override void RotateTo(Quaternion rotation) => target.rotation = rotation;

        /// <summary>
        /// Nothing to stop.
        /// </summary>
        public override void Stop()
        {
        }
    }
}
