namespace GridSystem.Samples.Tooltip
{
    using System;
    using Definitions;
    using MessagePipe;
    using Runtime.Composition;
    using Runtime.Messaging;
    using Runtime.Pointer;

    /// <summary>
    /// Shows tooltip of the hovered item when its definition has a description.
    /// </summary>
    public sealed class ItemTooltipPresenter : IGridInitializable, IDisposable
    {
        public ItemTooltipPresenter(ItemTooltipView view, IPointerInput input, ISubscriber<ItemHoverMessage> hoverSubscriber)
        {
            _view = view;
            _input = input;
            _hoverSubscriber = hoverSubscriber;
        }

        private readonly ItemTooltipView _view;
        private readonly IPointerInput _input;
        private readonly ISubscriber<ItemHoverMessage> _hoverSubscriber;
        private IDisposable _subscription;

        /// <summary>
        /// Subscribes to hover changes.
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
                _view.Show(description, _input.Position);
            }
            else
            {
                _view.Hide();
            }
        }
    }
}
