namespace GridSystem.Runtime.Surfaces
{
    using Core.Inventories;
    using Core.Items;
    using UnityEngine;

    /// <summary>
    /// Result of picking an opened surface by screen point.
    /// </summary>
    public readonly struct SurfaceHit
    {
        public SurfaceHit(GridSurface surface, GridInventory inventory, Vector2Int cell, GridItem item)
        {
            Surface = surface;
            Inventory = inventory;
            Cell = cell;
            Item = item;
        }

        /// <summary>
        /// Picked surface.
        /// </summary>
        public readonly GridSurface Surface;

        /// <summary>
        /// Inventory shown on the surface.
        /// </summary>
        public readonly GridInventory Inventory;

        /// <summary>
        /// Cell under the point.
        /// </summary>
        public readonly Vector2Int Cell;

        /// <summary>
        /// Item occupying the cell or null.
        /// </summary>
        public readonly GridItem Item;
    }
}
