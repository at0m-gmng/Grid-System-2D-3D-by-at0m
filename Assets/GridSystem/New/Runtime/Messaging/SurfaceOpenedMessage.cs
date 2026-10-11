namespace GridSystem.Runtime.Messaging
{
    using Core.Inventories;
    using Surfaces;

    /// <summary>
    /// Published after an inventory is shown on a surface.
    /// </summary>
    public readonly struct SurfaceOpenedMessage
    {
        public SurfaceOpenedMessage(GridSurface surface, GridInventory inventory)
        {
            Surface = surface;
            Inventory = inventory;
        }

        /// <summary>
        /// Surface view.
        /// </summary>
        public readonly GridSurface Surface;

        /// <summary>
        /// Shown inventory.
        /// </summary>
        public readonly GridInventory Inventory;
    }
}
