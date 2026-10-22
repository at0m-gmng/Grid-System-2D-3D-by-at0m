namespace GridSystem.Runtime.Interaction
{
    using System;
    using Composition;
    using Core.Inventories;
    using Core.Items;
    using MessagePipe;
    using Messaging;
    using Surfaces;
    using UnityEngine;

    /// <summary>
    /// Publishes hovered item changes while nothing is dragged. Works only on pointer and drag messages.
    /// </summary>
    public sealed class ItemHoverService : IGridInitializable, IDisposable
    {
        public ItemHoverService(ISurfaceLocator locator, ISubscriber<PointerMovedMessage> movedSubscriber,
            ISubscriber<ItemDragStartedMessage> dragStartedSubscriber, ISubscriber<ItemDragEndedMessage> dragEndedSubscriber,
            ISubscriber<ItemRemovedMessage> removedSubscriber, IPublisher<ItemHoverMessage> publisher)
        {
            _locator = locator;
            _movedSubscriber = movedSubscriber;
            _dragStartedSubscriber = dragStartedSubscriber;
            _dragEndedSubscriber = dragEndedSubscriber;
            _removedSubscriber = removedSubscriber;
            _publisher = publisher;
        }

        private readonly ISurfaceLocator _locator;
        private readonly ISubscriber<PointerMovedMessage> _movedSubscriber;
        private readonly ISubscriber<ItemDragStartedMessage> _dragStartedSubscriber;
        private readonly ISubscriber<ItemDragEndedMessage> _dragEndedSubscriber;
        private readonly ISubscriber<ItemRemovedMessage> _removedSubscriber;
        private readonly IPublisher<ItemHoverMessage> _publisher;
        private IDisposable _subscriptions;
        private GridItem _item;
        private Vector2 _position;
        private bool _isDragging;

        /// <summary>
        /// Subscribes to pointer, drag and item removal messages.
        /// </summary>
        public void Initialize()
        {
            _subscriptions = DisposableBag.Create(
                _movedSubscriber.Subscribe(OnMoved),
                _dragStartedSubscriber.Subscribe(OnDragStarted),
                _dragEndedSubscriber.Subscribe(OnDragEnded),
                _removedSubscriber.Subscribe(OnItemRemoved));
        }

        /// <summary>
        /// Unsubscribes.
        /// </summary>
        public void Dispose() => _subscriptions?.Dispose();

        private void OnMoved(PointerMovedMessage message)
        {
            _position = message.ScreenPosition;
            Evaluate();
        }

        private void OnDragStarted(ItemDragStartedMessage message)
        {
            _isDragging = true;
            Evaluate();
        }

        private void OnDragEnded(ItemDragEndedMessage message)
        {
            _isDragging = false;
            Evaluate();
        }

        private void OnItemRemoved(ItemRemovedMessage message)
        {
            if (message.Item == _item)
            {
                Evaluate();
            }
        }

        private void Evaluate()
        {
            GridItem item = null;
            GridInventory inventory = null;
            if (!_isDragging && _locator.TryPick(_position, out SurfaceHit hit))
            {
                item = hit.Item;
                inventory = hit.Inventory;
            }

            if (item != _item)
            {
                _item = item;
                _publisher.Publish(new ItemHoverMessage(inventory, item, _position));
            }
        }
    }
}
