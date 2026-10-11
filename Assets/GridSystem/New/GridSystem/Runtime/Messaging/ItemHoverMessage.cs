namespace GridSystem.Runtime.Messaging
{
    using Core.Inventories;
    using Core.Items;
    using UnityEngine;

    /// <summary>
    /// Published when the hovered item changes. Item is null when nothing is hovered.
    /// </summary>
    public readonly struct ItemHoverMessage
    {
        public ItemHoverMessage(GridInventory inventory, GridItem item, Vector2 screenPosition)
        {
            Inventory = inventory;
            Item = item;
            ScreenPosition = screenPosition;
        }

        /// <summary>
        /// Inventory of the hovered item.
        /// </summary>
        public readonly GridInventory Inventory;

        /// <summary>
        /// Hovered item or null.
        /// </summary>
        public readonly GridItem Item;

        /// <summary>
        /// Pointer screen position.
        /// </summary>
        public readonly Vector2 ScreenPosition;
    }
}
