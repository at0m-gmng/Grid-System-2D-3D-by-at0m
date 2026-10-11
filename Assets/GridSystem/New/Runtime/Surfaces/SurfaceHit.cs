namespace GridSystem.Runtime.Surfaces
{
    using Core.Items;
    using UnityEngine;

    /// <summary>
    /// Result of picking an opened surface by screen point.
    /// </summary>
    public readonly struct SurfaceHit
    {
        public SurfaceHit(GridSurfacePresenter presenter, Vector2Int cell, GridItem item)
        {
            Presenter = presenter;
            Cell = cell;
            Item = item;
        }

        /// <summary>
        /// Picked surface presenter.
        /// </summary>
        public readonly GridSurfacePresenter Presenter;

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
