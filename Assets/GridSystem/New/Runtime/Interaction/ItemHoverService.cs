namespace GridSystem.Runtime.Interaction
{
    using System;
    using Composition;
    using Core.Inventories;
    using Core.Items;
    using MessagePipe;
    using Messaging;
    using Pointer;
    using R3;
    using Surfaces;

    /// <summary>
    /// Publishes hovered item changes while nothing is dragged.
    /// </summary>
    public sealed class ItemHoverService : IGridInitializable, IDisposable
    {
        public ItemHoverService(IPointerInput input, ISurfaceLocator locator, IItemDragService dragService, IPublisher<ItemHoverMessage> publisher)
        {
            _input = input;
            _locator = locator;
            _dragService = dragService;
            _publisher = publisher;
        }

        private readonly IPointerInput _input;
        private readonly ISurfaceLocator _locator;
        private readonly IItemDragService _dragService;
        private readonly IPublisher<ItemHoverMessage> _publisher;
        private IDisposable _subscription;
        private GridItem _item;

        /// <summary>
        /// Starts tracking.
        /// </summary>
        public void Initialize() => _subscription = Observable.EveryUpdate().Subscribe(this, static (_, service) => service.OnUpdate());

        /// <summary>
        /// Stops tracking.
        /// </summary>
        public void Dispose() => _subscription.Dispose();

        private void OnUpdate()
        {
            GridItem item = null;
            GridInventory inventory = null;
            if (!_dragService.IsDragging && _locator.TryPick(_input.Position, out SurfaceHit hit))
            {
                item = hit.Item;
                inventory = hit.Presenter.Inventory;
            }

            if (item != _item)
            {
                _item = item;
                _publisher.Publish(new ItemHoverMessage(inventory, item));
            }
        }
    }
}
