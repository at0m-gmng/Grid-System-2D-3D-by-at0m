namespace GridSystem.Runtime.Messaging
{
    using UnityEngine;

    /// <summary>
    /// Published when the pointer button is pressed or released.
    /// </summary>
    public readonly struct PointerButtonMessage
    {
        public PointerButtonMessage(Vector2 screenPosition, bool isPressed)
        {
            ScreenPosition = screenPosition;
            IsPressed = isPressed;
        }

        /// <summary>
        /// Pointer screen position.
        /// </summary>
        public readonly Vector2 ScreenPosition;

        /// <summary>
        /// True on press, false on release.
        /// </summary>
        public readonly bool IsPressed;
    }
}
