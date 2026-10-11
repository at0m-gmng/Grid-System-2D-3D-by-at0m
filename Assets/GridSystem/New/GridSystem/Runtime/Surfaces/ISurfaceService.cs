namespace GridSystem.Runtime.Surfaces
{
    using Core.Inventories;

    /// <summary>
    /// Shows inventories on surfaces.
    /// </summary>
    public interface ISurfaceService
    {
        /// <summary>
        /// Shows the inventory on the surface replacing the previously shown one.
        /// </summary>
        public void Open(GridSurface surface, GridInventory inventory);

        /// <summary>
        /// Hides the inventory shown on the surface.
        /// </summary>
        public void Close(GridSurface surface);

        /// <summary>
        /// Returns true when the surface shows an inventory.
        /// </summary>
        public bool IsOpen(GridSurface surface);
    }
}
