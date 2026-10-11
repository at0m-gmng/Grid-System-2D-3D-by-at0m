namespace GridSystem.Runtime.Surfaces
{
    using Core.Inventories;
    using MessagePipe;
    using Messaging;
    using Views;

    /// <summary>
    /// Creates presenters with their message channels.
    /// </summary>
    public sealed class GridSurfacePresenterFactory
    {
        public GridSurfacePresenterFactory(ItemViewPool pool, ISubscriber<ItemDragStartedMessage> startedSubscriber,
            ISubscriber<ItemDragUpdatedMessage> updatedSubscriber, ISubscriber<ItemDragEndedMessage> endedSubscriber)
        {
            _pool = pool;
            _startedSubscriber = startedSubscriber;
            _updatedSubscriber = updatedSubscriber;
            _endedSubscriber = endedSubscriber;
        }

        private readonly ItemViewPool _pool;
        private readonly ISubscriber<ItemDragStartedMessage> _startedSubscriber;
        private readonly ISubscriber<ItemDragUpdatedMessage> _updatedSubscriber;
        private readonly ISubscriber<ItemDragEndedMessage> _endedSubscriber;

        /// <summary>
        /// Creates presenter showing the inventory on the surface.
        /// </summary>
        public GridSurfacePresenter Create(GridSurface surface, GridInventory inventory)
            => new GridSurfacePresenter(surface, inventory, _pool, _startedSubscriber, _updatedSubscriber, _endedSubscriber);
    }
}
