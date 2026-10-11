namespace GridSystem.Runtime.Messaging
{
    using Core.Inventories;
    using Surfaces;

    /// <summary>
    /// Published after an inventory is hidden from a surface.
    /// </summary>
    public readonly struct SurfaceClosedMessage
    {
        public SurfaceClosedMessage(GridSurface surface, GridInventory inventory)
        {
            Surface = surface;
            Inventory = inventory;
        }

        /// <summary>
        /// Surface view.
        /// </summary>
        public readonly GridSurface Surface;

        /// <summary>
        /// Hidden inventory.
        /// </summary>
        public readonly GridInventory Inventory;
    }
}
