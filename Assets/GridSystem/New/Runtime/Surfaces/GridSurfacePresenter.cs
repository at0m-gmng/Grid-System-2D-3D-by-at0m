namespace GridSystem.Runtime.Surfaces
{
    using System;
    using System.Collections.Generic;
    using Core.Inventories;
    using Core.Items;
    using R3;
    using UnityEngine;
    using Views;

    /// <summary>
    /// Keeps item views of one surface in sync with one inventory.
    /// </summary>
    public sealed class GridSurfacePresenter : IDisposable
    {
        public GridSurfacePresenter(GridSurface surface, GridInventory inventory, ItemViewPool pool)
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
                inventory.Moved.Subscribe(this, static (movement, presenter) => presenter.Move(movement.Item)));
        }

        private readonly ItemViewPool _pool;
        private readonly IDisposable _subscriptions;
        private readonly Dictionary<int, ItemView> _views = new();
        private int _arrivalItemId;
        private Pose _arrivalPose;

        /// <summary>
        /// Presented surface.
        /// </summary>
        public GridSurface Surface { get; }

        /// <summary>
        /// Presented inventory.
        /// </summary>
        public GridInventory Inventory { get; }

        /// <summary>
        /// Finds view of a placed item.
        /// </summary>
        public bool TryGetView(int itemId, out ItemView view) => _views.TryGetValue(itemId, out view);

        /// <summary>
        /// Shows or hides view of a placed item.
        /// </summary>
        public void SetItemVisible(int itemId, bool isVisible)
        {
            if (_views.TryGetValue(itemId, out ItemView view))
            {
                view.SetVisible(isVisible);
            }
        }

        /// <summary>
        /// Shows view of a placed item animating it from the pose back to its cell.
        /// </summary>
        public void ReturnItem(GridItem item, Pose fromPose)
        {
            if (_views.TryGetValue(item.Id, out ItemView view))
            {
                view.SetVisible(true);
                view.Snap(fromPose);
                view.MoveTo(Surface.GetItemPose(item.Origin, item.Rotation));
            }
        }

        /// <summary>
        /// Rents a free view of the definition, used as drag preview.
        /// </summary>
        public ItemView RentPreview(IGridItemDefinition definition) => RentView(definition);

        /// <summary>
        /// Returns preview view to the pool.
        /// </summary>
        public void ReleasePreview(ItemView view) => _pool.Return(view);

        /// <summary>
        /// Next spawn or move of the item starts from the pose instead of its cell.
        /// </summary>
        public void ExpectArrival(int itemId, Pose pose)
        {
            _arrivalItemId = itemId;
            _arrivalPose = pose;
        }

        /// <summary>
        /// Cancels expected arrival.
        /// </summary>
        public void ClearArrival() => _arrivalItemId = 0;

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
            if (!TryArrive(view, item))
            {
                view.Snap(Surface.GetItemPose(item.Origin, item.Rotation));
            }
        }

        private void Despawn(GridItem item)
        {
            if (_views.Remove(item.Id, out ItemView view))
            {
                _pool.Return(view);
            }
        }

        private void Move(GridItem item)
        {
            ItemView view = _views[item.Id];
            view.SetVisible(true);
            if (!TryArrive(view, item))
            {
                view.MoveTo(Surface.GetItemPose(item.Origin, item.Rotation));
            }
        }

        private bool TryArrive(ItemView view, GridItem item)
        {
            bool result = _arrivalItemId == item.Id;
            if (result)
            {
                _arrivalItemId = 0;
                view.Snap(_arrivalPose);
                view.MoveTo(Surface.GetItemPose(item.Origin, item.Rotation));
            }

            return result;
        }

        private ItemView RentView(IGridItemDefinition definition)
        {
            ItemView view = _pool.Rent(Surface.Views.GetPrefab(definition), Surface.ItemsRoot);
            view.Setup(definition, Surface);
            return view;
        }
    }
}
