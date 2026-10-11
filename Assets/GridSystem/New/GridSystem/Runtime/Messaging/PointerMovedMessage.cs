namespace GridSystem.Runtime.Messaging
{
    using UnityEngine;

    /// <summary>
    /// Published when the pointer screen position changes.
    /// </summary>
    public readonly struct PointerMovedMessage
    {
        public PointerMovedMessage(Vector2 screenPosition)
        {
            ScreenPosition = screenPosition;
        }

        /// <summary>
        /// Pointer screen position.
        /// </summary>
        public readonly Vector2 ScreenPosition;
    }
}
