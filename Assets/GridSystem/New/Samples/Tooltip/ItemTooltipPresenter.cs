namespace GridSystem.Samples.Tooltip
{
    using System;
    using Definitions;
    using MessagePipe;
    using Runtime.Composition;
    using Runtime.Messaging;

    /// <summary>
    /// Shows tooltip of the hovered item when its definition has a description.
    /// </summary>
    public sealed class ItemTooltipPresenter : IGridInitializable, IDisposable
    {
        public ItemTooltipPresenter(Bootstrap.SampleContent content, ISubscriber<ItemHoverMessage> hoverSubscriber)
        {
            _content = content;
            _hoverSubscriber = hoverSubscriber;
        }

        private readonly Bootstrap.SampleContent _content;
        private readonly ISubscriber<ItemHoverMessage> _hoverSubscriber;
        private IDisposable _subscription;

        /// <summary>
        /// Subscribes to hover changes.
        /// </summary>
        public void Initialize()
        {
            _content.Ui.Tooltip.Hide();
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
                _content.Ui.Tooltip.Show(description, message.ScreenPosition);
            }
            else
            {
                _content.Ui.Tooltip.Hide();
            }
        }
    }
}
