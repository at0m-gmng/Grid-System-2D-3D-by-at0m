namespace GridSystem.Samples.Description
{
    using System;
    using Definitions;
    using MessagePipe;
    using Runtime.Composition;
    using Runtime.Messaging;

    /// <summary>
    /// Shows description of the hovered item when its definition has one.
    /// </summary>
    public sealed class ItemDescriptionPresenter : IGridInitializable, IDisposable
    {
        public ItemDescriptionPresenter(ItemDescriptionView view, ISubscriber<ItemHoverMessage> hoverSubscriber)
        {
            _view = view;
            _hoverSubscriber = hoverSubscriber;
        }

        private readonly ItemDescriptionView _view;
        private readonly ISubscriber<ItemHoverMessage> _hoverSubscriber;
        private IDisposable _subscription;

        /// <summary>
        /// Hides the popup and subscribes to hover changes.
        /// </summary>
        public void Initialize()
        {
            _view.Hide();
            _subscription = _hoverSubscriber.Subscribe(OnHover);
        }

        /// <summary>
        /// Unsubscribes.
        /// </summary>
        public void Dispose() => _subscription.Dispose();

        private void OnHover(ItemHoverMessage message)
        {
            if (message.Item != null && message.Item.Definition is IItemDescription description)
            {
                _view.Show(description);
            }
            else
            {
                _view.Hide();
            }
        }
    }
}
