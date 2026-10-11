namespace GridSystem.Runtime.Messaging
{
    using Core.Inventories;
    using Core.Items;

    /// <summary>
    /// Published after two items are merged into a new one.
    /// </summary>
    public readonly struct ItemsMergedMessage
    {
        public ItemsMergedMessage(GridInventory inventory, GridItem first, GridItem second, GridItem result)
        {
            Inventory = inventory;
            First = first;
            Second = second;
            Result = result;
        }

        /// <summary>
        /// Inventory containing the result.
        /// </summary>
        public readonly GridInventory Inventory;

        /// <summary>
        /// Dragged item.
        /// </summary>
        public readonly GridItem First;

        /// <summary>
        /// Target item.
        /// </summary>
        public readonly GridItem Second;

        /// <summary>
        /// Produced item.
        /// </summary>
        public readonly GridItem Result;
    }
}
