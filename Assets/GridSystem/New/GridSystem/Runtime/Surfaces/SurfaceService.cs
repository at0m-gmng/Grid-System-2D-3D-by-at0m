namespace GridSystem.Runtime.Surfaces
{
    using System;
    using System.Collections.Generic;
    using Core.Inventories;
    using Core.Items;
    using MessagePipe;
    using Messaging;
    using UnityEngine;

    /// <summary>
    /// Owns surface presenters and picks surfaces by screen point.
    /// </summary>
    internal sealed class SurfaceService : ISurfaceService, ISurfaceLocator, IDisposable
    {
        public SurfaceService(GridSurfacePresenterFactory factory, IPublisher<SurfaceOpenedMessage> openedPublisher,
            IPublisher<SurfaceClosedMessage> closedPublisher)
        {
            _factory = factory;
            _openedPublisher = openedPublisher;
            _closedPublisher = closedPublisher;
        }

        private readonly GridSurfacePresenterFactory _factory;
        private readonly IPublisher<SurfaceOpenedMessage> _openedPublisher;
        private readonly IPublisher<SurfaceClosedMessage> _closedPublisher;
        private readonly List<GridSurfacePresenter> _presenters = new();

        /// <summary>
        /// Shows the inventory on the surface replacing the previously shown one.
        /// </summary>
        public void Open(GridSurface surface, GridInventory inventory)
        {
            Close(surface);
            GridSurfacePresenter presenter = _factory.Create(surface, inventory);
            int index = 0;
            while (index < _presenters.Count && _presenters[index].Surface.Priority >= surface.Priority)
            {
                index++;
            }

            _presenters.Insert(index, presenter);
            _openedPublisher.Publish(new SurfaceOpenedMessage(surface, inventory));
        }

        /// <summary>
        /// Hides the inventory shown on the surface.
        /// </summary>
        public void Close(GridSurface surface)
        {
            int index = IndexOf(surface);
            if (index >= 0)
            {
                GridSurfacePresenter presenter = _presenters[index];
                _presenters.RemoveAt(index);
                presenter.Dispose();
                _closedPublisher.Publish(new SurfaceClosedMessage(surface, presenter.Inventory));
            }
        }

        /// <summary>
        /// Returns true when the surface shows an inventory.
        /// </summary>
        public bool IsOpen(GridSurface surface) => IndexOf(surface) >= 0;

        /// <summary>
        /// Picks the highest priority opened surface containing the point.
        /// </summary>
        public bool TryPick(Vector2 screenPosition, out SurfaceHit hit)
        {
            hit = default;
            bool isFound = false;
            for (int i = 0; !isFound && i < _presenters.Count; i++)
            {
                GridSurfacePresenter presenter = _presenters[i];
                if (presenter.Surface.TryGetCell(screenPosition, out Vector2Int cell))
                {
                    isFound = true;
                    presenter.Inventory.TryGetItemAt(cell, out GridItem item);
                    hit = new SurfaceHit(presenter.Surface, presenter.Inventory, cell, item);
                }
            }

            return isFound;
        }

        /// <summary>
        /// Unbinds every presenter. Views are destroyed with the scene.
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < _presenters.Count; i++)
            {
                _presenters[i].Unbind();
            }

            _presenters.Clear();
        }

        private int IndexOf(GridSurface surface)
        {
            int index = -1;
            for (int i = 0; index < 0 && i < _presenters.Count; i++)
            {
                if (_presenters[i].Surface == surface)
                {
                    index = i;
                }
            }

            return index;
        }
    }
}
