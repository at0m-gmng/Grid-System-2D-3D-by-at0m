namespace GridSystem.Runtime.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Composition;
    using Core.Inventories;
    using Core.Items;
    using Core.Shapes;
    using Drop;
    using MessagePipe;
    using Messaging;
    using Surfaces;
    using UnityEngine;

    /// <summary>
    /// Drag and drop state machine. Listens to pointer messages, evaluates drop handlers
    /// only when the target changes and publishes drag messages. Knows nothing about views.
    /// </summary>
    public sealed class ItemDragService : IGridInitializable, IDisposable
    {
        public ItemDragService(ISurfaceLocator locator, IEnumerable<IItemDropHandler> handlers,
            ISubscriber<PointerButtonMessage> buttonSubscriber, ISubscriber<PointerMovedMessage> movedSubscriber,
            ISubscriber<RotateRequestedMessage> rotateSubscriber, ISubscriber<SurfaceClosedMessage> closedSubscriber,
            ISubscriber<ItemRemovedMessage> removedSubscriber,
            IPublisher<ItemDragStartedMessage> startedPublisher, IPublisher<ItemDragUpdatedMessage> updatedPublisher,
            IPublisher<ItemDragEndedMessage> endedPublisher)
        {
            _locator = locator;
            _handlers = handlers.OrderBy(handler => handler.Order).ToArray();
            _buttonSubscriber = buttonSubscriber;
            _movedSubscriber = movedSubscriber;
            _rotateSubscriber = rotateSubscriber;
            _closedSubscriber = closedSubscriber;
            _removedSubscriber = removedSubscriber;
            _startedPublisher = startedPublisher;
            _updatedPublisher = updatedPublisher;
            _endedPublisher = endedPublisher;
        }

        private readonly ISurfaceLocator _locator;
        private readonly IItemDropHandler[] _handlers;
        private readonly ISubscriber<PointerButtonMessage> _buttonSubscriber;
        private readonly ISubscriber<PointerMovedMessage> _movedSubscriber;
        private readonly ISubscriber<RotateRequestedMessage> _rotateSubscriber;
        private readonly ISubscriber<SurfaceClosedMessage> _closedSubscriber;
        private readonly ISubscriber<ItemRemovedMessage> _removedSubscriber;
        private readonly IPublisher<ItemDragStartedMessage> _startedPublisher;
        private readonly IPublisher<ItemDragUpdatedMessage> _updatedPublisher;
        private readonly IPublisher<ItemDragEndedMessage> _endedPublisher;
        private IDisposable _subscriptions;
        private GridItem _item;
        private GridInventory _source;
        private GridSurface _sourceSurface;
        private GridSurface _surface;
        private Vector2 _position;
        private int _rotation;
        private bool _hasTarget;
        private SurfaceHit _target;
        private DropContext _context;
        private IItemDropHandler _handler;

        private bool IsDragging => _item != null;

        /// <summary>
        /// Subscribes to pointer and surface messages.
        /// </summary>
        public void Initialize()
        {
            _subscriptions = DisposableBag.Create(
                _buttonSubscriber.Subscribe(OnButton),
                _movedSubscriber.Subscribe(OnMoved),
                _rotateSubscriber.Subscribe(OnRotate),
                _closedSubscriber.Subscribe(OnSurfaceClosed),
                _removedSubscriber.Subscribe(OnItemRemoved));
        }

        /// <summary>
        /// Unsubscribes.
        /// </summary>
        public void Dispose() => _subscriptions?.Dispose();

        private void OnButton(PointerButtonMessage message)
        {
            _position = message.ScreenPosition;
            if (message.IsPressed)
            {
                TryBegin();
            }
            else if (IsDragging)
            {
                End(_handler != null && _handler.CanHandle(_context));
            }
        }

        private void OnMoved(PointerMovedMessage message)
        {
            _position = message.ScreenPosition;
            if (IsDragging)
            {
                Evaluate();
                PublishUpdate();
            }
        }

        private void OnRotate(RotateRequestedMessage message)
        {
            if (IsDragging && _item.Definition.IsRotatable)
            {
                _rotation = GridShape.NormalizeRotation(_rotation + 1);
                _hasTarget = false;
                Evaluate();
                PublishUpdate();
            }
        }

        private void OnSurfaceClosed(SurfaceClosedMessage message)
        {
            if (IsDragging && (message.Surface == _surface || message.Surface == _sourceSurface))
            {
                End(false);
            }
        }

        private void OnItemRemoved(ItemRemovedMessage message)
        {
            if (IsDragging && message.Item == _item)
            {
                End(false);
            }
        }

        private void TryBegin()
        {
            if (!IsDragging && _locator.TryPick(_position, out SurfaceHit hit) && hit.Item != null)
            {
                _item = hit.Item;
                _source = hit.Inventory;
                _sourceSurface = hit.Surface;
                _surface = hit.Surface;
                _rotation = hit.Item.Rotation;
                _hasTarget = false;
                _startedPublisher.Publish(new ItemDragStartedMessage(_item, _source, _sourceSurface));
                Evaluate();
                PublishUpdate();
            }
        }

        private void Evaluate()
        {
            bool hasHit = _locator.TryPick(_position, out SurfaceHit hit) && hit.Surface.CanDisplay(_item.Definition);
            if (hasHit && (!_hasTarget || IsTargetChanged(hit)))
            {
                _surface = hit.Surface;
                _target = hit;
                _context = new DropContext(_item, _source, hit.Inventory, hit.Surface, hit.Cell, _rotation, hit.Item);
                _handler = FindHandler(_context);
            }
            else if (!hasHit)
            {
                _handler = null;
            }

            _hasTarget = hasHit;
        }

        private bool IsTargetChanged(in SurfaceHit hit) => hit.Surface != _target.Surface || hit.Cell != _target.Cell || hit.Item != _target.Item;

        private void PublishUpdate() => _updatedPublisher.Publish(new ItemDragUpdatedMessage(_item, _surface, _position, _rotation, _handler != null));

        private void End(bool isDropped)
        {
            GridItem item = _item;
            GridInventory source = _source;
            GridSurface sourceSurface = _sourceSurface;
            GridSurface surface = _surface;
            IItemDropHandler handler = _handler;
            DropContext context = _context;
            _item = null;
            _source = null;
            _sourceSurface = null;
            _surface = null;
            _handler = null;
            _hasTarget = false;
            _target = default;
            _context = default;
            if (isDropped)
            {
                handler.Handle(context);
            }

            _endedPublisher.Publish(new ItemDragEndedMessage(item, source, sourceSurface, surface, isDropped));
        }

        private IItemDropHandler FindHandler(in DropContext context)
        {
            IItemDropHandler result = null;
            for (int i = 0; result == null && i < _handlers.Length; i++)
            {
                if (_handlers[i].CanHandle(context))
                {
                    result = _handlers[i];
                }
            }

            return result;
        }
    }
}
