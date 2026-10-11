namespace GridSystem.Runtime.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Composition;
    using Core.Items;
    using Core.Shapes;
    using Drop;
    using MessagePipe;
    using Messaging;
    using Pointer;
    using R3;
    using Surfaces;
    using UnityEngine;
    using Views;

    /// <summary>
    /// Pointer driven drag and drop between any opened surfaces, including UI and World.
    /// The dragged item stays in its inventory until a drop handler accepts the drop.
    /// </summary>
    public sealed class ItemDragService : IItemDragService, IGridInitializable, IDisposable
    {
        public ItemDragService(IPointerInput input, ISurfaceLocator locator, IEnumerable<IItemDropHandler> handlers,
            IPublisher<ItemDragStartedMessage> startedPublisher, IPublisher<ItemDragEndedMessage> endedPublisher,
            ISubscriber<SurfaceClosedMessage> closedSubscriber)
        {
            _input = input;
            _locator = locator;
            _closedSubscriber = closedSubscriber;
            _handlers = handlers.OrderBy(handler => handler.Order).ToArray();
            _startedPublisher = startedPublisher;
            _endedPublisher = endedPublisher;
        }

        private readonly IPointerInput _input;
        private readonly ISurfaceLocator _locator;
        private readonly IItemDropHandler[] _handlers;
        private readonly IPublisher<ItemDragStartedMessage> _startedPublisher;
        private readonly IPublisher<ItemDragEndedMessage> _endedPublisher;
        private readonly ISubscriber<SurfaceClosedMessage> _closedSubscriber;
        private IDisposable _subscriptions;
        private GridItem _item;
        private GridSurfacePresenter _source;
        private GridSurfacePresenter _owner;
        private ItemView _preview;
        private IItemDropHandler _handler;
        private DropContext _context;
        private int _rotation;

        /// <summary>
        /// True while an item is dragged.
        /// </summary>
        public bool IsDragging { get; private set; }

        /// <summary>
        /// Subscribes to pointer events.
        /// </summary>
        public void Initialize()
        {
            _subscriptions = Disposable.Combine(
                _input.Pressed.Subscribe(this, static (_, service) => service.OnPressed()),
                _input.Released.Subscribe(this, static (_, service) => service.OnReleased()),
                _input.Rotated.Subscribe(this, static (_, service) => service.OnRotated()),
                Observable.EveryUpdate().Subscribe(this, static (_, service) => service.OnUpdate()),
                _closedSubscriber.Subscribe(OnSurfaceClosed));
        }

        /// <summary>
        /// Cancels the current drag without dropping.
        /// </summary>
        public void Cancel()
        {
            if (IsDragging)
            {
                _handler = null;
                End();
            }
        }

        /// <summary>
        /// Unsubscribes from pointer events.
        /// </summary>
        public void Dispose() => _subscriptions.Dispose();

        private void OnPressed()
        {
            if (!IsDragging && _locator.TryPick(_input.Position, out SurfaceHit hit) && hit.Item != null)
            {
                Begin(hit);
            }
        }

        private void OnReleased()
        {
            if (IsDragging)
            {
                End();
            }
        }

        private void OnRotated()
        {
            if (IsDragging && _item.Definition.IsRotatable)
            {
                _rotation = GridShape.NormalizeRotation(_rotation + 1);
                _preview.RotateTo(_owner.Surface.GetRotation(_rotation));
                UpdateTarget();
            }
        }

        private void OnSurfaceClosed(SurfaceClosedMessage message)
        {
            if (IsDragging && (message.Surface == _owner.Surface || message.Surface == _source.Surface))
            {
                Cancel();
            }
        }

        private void OnUpdate()
        {
            if (IsDragging)
            {
                UpdateTarget();
            }
        }

        private void Begin(in SurfaceHit hit)
        {
            IsDragging = true;
            _item = hit.Item;
            _source = hit.Presenter;
            _rotation = _item.Rotation;
            _source.SetItemVisible(_item.Id, false);
            _source.TryGetView(_item.Id, out ItemView view);
            AttachPreview(_source, view.Pose.position);
            _startedPublisher.Publish(new ItemDragStartedMessage(_source.Inventory, _item));
            UpdateTarget();
        }

        private void UpdateTarget()
        {
            Vector2 pointer = _input.Position;
            bool hasHit = _locator.TryPick(pointer, out SurfaceHit hit);
            if (hasHit && hit.Presenter != _owner)
            {
                _owner.ReleasePreview(_preview);
                AttachPreview(hit.Presenter, hit.Presenter.Surface.GetCellPosition(hit.Cell));
            }

            if (_owner.Surface.TryGetWorldPoint(pointer, out Vector3 point))
            {
                _preview.Follow(point);
            }

            _handler = null;
            if (hasHit)
            {
                _context = new DropContext(_item, _source.Inventory, hit.Presenter.Inventory, hit.Cell, _rotation, hit.Item);
                _handler = FindHandler(_context);
            }

            _preview.SetPlacementState(_handler != null ? PlacementState.Valid : PlacementState.Invalid);
        }

        private void End()
        {
            Pose pose = _preview.Pose;
            bool isDropped = _handler != null;
            GridSurfacePresenter owner = _owner;
            owner.ReleasePreview(_preview);
            if (isDropped)
            {
                owner.ExpectArrival(_item.Id, pose);
                _handler.Handle(_context);
                owner.ClearArrival();
                _source.SetItemVisible(_item.Id, true);
            }
            else if (owner == _source)
            {
                _source.ReturnItem(_item, pose);
            }
            else
            {
                _source.SetItemVisible(_item.Id, true);
            }

            _endedPublisher.Publish(new ItemDragEndedMessage(_source.Inventory, _item, isDropped));
            IsDragging = false;
            _item = null;
            _source = null;
            _owner = null;
            _preview = null;
            _handler = null;
            _context = default;
        }

        private void AttachPreview(GridSurfacePresenter owner, Vector3 position)
        {
            _owner = owner;
            _preview = owner.RentPreview(_item.Definition);
            _preview.Snap(new Pose(position, owner.Surface.GetRotation(_rotation)));
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
