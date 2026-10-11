namespace GridSystem.Runtime.Pointer
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// Device independent pointer source.
    /// </summary>
    public interface IPointerInput
    {
        /// <summary>
        /// Current screen position.
        /// </summary>
        public Vector2 Position { get; }

        /// <summary>
        /// Raised on press.
        /// </summary>
        public Observable<Unit> Pressed { get; }

        /// <summary>
        /// Raised on release.
        /// </summary>
        public Observable<Unit> Released { get; }

        /// <summary>
        /// Raised on rotate request.
        /// </summary>
        public Observable<Unit> Rotated { get; }
    }
}
