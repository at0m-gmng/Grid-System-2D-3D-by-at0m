namespace GridSystem.Runtime.Messaging
{
    using Core.Items;
    using Surfaces;
    using UnityEngine;

    /// <summary>
    /// Published when pointer, rotation or drop target of the dragged item changes.
    /// </summary>
    public readonly struct ItemDragUpdatedMessage
    {
        public ItemDragUpdatedMessage(GridItem item, GridSurface surface, Vector2 screenPosition, int rotation, bool isValid)
        {
            Item = item;
            Surface = surface;
            ScreenPosition = screenPosition;
            Rotation = rotation;
            IsValid = isValid;
        }

        /// <summary>
        /// Dragged item.
        /// </summary>
        public readonly GridItem Item;

        /// <summary>
        /// Hovered surface or the last hovered one.
        /// </summary>
        public readonly GridSurface Surface;

        /// <summary>
        /// Pointer screen position.
        /// </summary>
        public readonly Vector2 ScreenPosition;

        /// <summary>
        /// Desired clockwise quarter turns.
        /// </summary>
        public readonly int Rotation;

        /// <summary>
        /// True when a drop handler accepts the current target.
        /// </summary>
        public readonly bool IsValid;
    }
}
