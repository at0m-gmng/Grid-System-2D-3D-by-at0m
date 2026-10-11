namespace GridSystem.Runtime.Surfaces
{
    using System;
    using System.Collections.Generic;
    using Core.Inventories;
    using Core.Items;
    using MessagePipe;
    using Messaging;
    using R3;
    using UnityEngine;
    using Views;

    /// <summary>
    /// Keeps item views of one surface in sync with one inventory and draws the drag preview on this surface.
    /// Reacts only to inventory events and drag messages, nobody calls it directly.
    /// </summary>
    public sealed class GridSurfacePresenter : IDisposable
    {
        public GridSurfacePresenter(GridSurface surface, GridInventory inventory, ItemViewPool pool,
            ISubscriber<ItemDragStartedMessage> startedSubscriber, ISubscriber<ItemDragUpdatedMessage> updatedSubscriber,
            ISubscriber<ItemDragEndedMessage> endedSubscriber)
        {
            Surface = surface;
            Inventory = inventory;
            _pool = pool;
            surface.Build(inventory.Model.Width, inventory.Model.Height);
            foreach (GridItem item in inventory.Items)
            {
                Spawn(item);
            }

            _subscriptions = Disposable.Combine(
                inventory.Added.Subscribe(this, static (item, presenter) => presenter.Spawn(item)),
                inventory.Removed.Subscribe(this, static (item, presenter) => presenter.Despawn(item)),
                inventory.Moved.Subscribe(this, static (movement, presenter) => presenter.Move(movement.Item)),
                startedSubscriber.Subscribe(OnDragStarted),
                updatedSubscriber.Subscribe(OnDragUpdated),
                endedSubscriber.Subscribe(OnDragEnded));
        }

        private readonly ItemViewPool _pool;
        private readonly IDisposable _subscriptions;
        private readonly Dictionary<int, ItemView> _views = new();
        private ItemView _preview;
        private int _previewRotation;

        /// <summary>
        /// Presented surface.
        /// </summary>
        public GridSurface Surface { get; }

        /// <summary>
        /// Presented inventory.
        /// </summary>
        public GridInventory Inventory { get; }

        /// <summary>
        /// Stops synchronization keeping views untouched. Used on scene teardown.
        /// </summary>
        public void Unbind() => _subscriptions.Dispose();

        /// <summary>
        /// Stops synchronization and returns all views to the pool.
        /// </summary>
        public void Dispose()
        {
            Unbind();
            ReleasePreview();
            foreach (ItemView view in _views.Values)
            {
                _pool.Return(view);
            }

            _views.Clear();
        }

        private void Spawn(GridItem item)
        {
            ItemView view = RentView(item.Definition);
            _views.Add(item.Id, view);
            view.Snap(GetPose(item));
        }

        private void Despawn(GridItem item)
        {
            if (_views.Remove(item.Id, out ItemView view))
            {
                _pool.Return(view);
            }
        }

        private void Move(GridItem item) => _views[item.Id].MoveTo(GetPose(item));

        private void OnDragStarted(ItemDragStartedMessage message)
        {
            if (message.SourceSurface == Surface)
            {
                _views[message.Item.Id].SetVisible(false);
            }
        }

        private void OnDragUpdated(ItemDragUpdatedMessage message)
        {
            if (message.Surface == Surface)
            {
                UpdatePreview(message);
            }
            else
            {
                ReleasePreview();
            }
        }

        private void OnDragEnded(ItemDragEndedMessage message)
        {
            bool hasView = _views.TryGetValue(message.Item.Id, out ItemView view);
            if (hasView)
            {
                view.SetVisible(true);
            }

            if (_preview != null)
            {
                Pose pose = _preview.Pose;
                ReleasePreview();
                if (hasView)
                {
                    view.Snap(pose);
                    view.MoveTo(GetPose(message.Item));
                }
            }
        }

        private void UpdatePreview(in ItemDragUpdatedMessage message)
        {
            bool hasPoint = Surface.TryGetWorldPoint(message.ScreenPosition, out Vector3 point);
            if (_preview == null)
            {
                _preview = RentView(message.Item.Definition);
                _previewRotation = message.Rotation;
                _preview.Snap(new Pose(point, Surface.GetRotation(message.Rotation)));
            }

            if (_previewRotation != message.Rotation)
            {
                _previewRotation = message.Rotation;
                _preview.RotateTo(Surface.GetRotation(message.Rotation));
            }

            if (hasPoint)
            {
                _preview.Follow(point);
            }

            _preview.SetPlacementState(message.IsValid ? PlacementState.Valid : PlacementState.Invalid);
        }

        private void ReleasePreview()
        {
            if (_preview != null)
            {
                _pool.Return(_preview);
                _preview = null;
            }
        }

        private ItemView RentView(IGridItemDefinition definition)
        {
            ItemView view = _pool.Rent(Surface.Views.GetPrefab(definition), Surface.ItemsRoot);
            view.Setup(definition, Surface);
            return view;
        }

        private Pose GetPose(GridItem item) => Surface.GetItemPose(item.Origin, item.Rotation);
    }
}
