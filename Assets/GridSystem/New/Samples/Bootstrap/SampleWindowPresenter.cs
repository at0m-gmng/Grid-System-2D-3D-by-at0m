namespace GridSystem.Samples.Bootstrap
{
    using System;
    using MessagePipe;
    using Runtime.Composition;
    using Runtime.Messaging;
    using Runtime.Surfaces;

    /// <summary>
    /// Shows window roots while their surfaces are opened.
    /// </summary>
    public sealed class SampleWindowPresenter : IGridInitializable, IDisposable
    {
        public SampleWindowPresenter(SampleSceneData data, ISurfaceService surfaces, ISubscriber<SurfaceOpenedMessage> openedSubscriber,
            ISubscriber<SurfaceClosedMessage> closedSubscriber)
        {
            _data = data;
            _surfaces = surfaces;
            _openedSubscriber = openedSubscriber;
            _closedSubscriber = closedSubscriber;
        }

        private readonly SampleSceneData _data;
        private readonly ISurfaceService _surfaces;
        private readonly ISubscriber<SurfaceOpenedMessage> _openedSubscriber;
        private readonly ISubscriber<SurfaceClosedMessage> _closedSubscriber;
        private IDisposable _subscriptions;

        /// <summary>
        /// Syncs window roots and subscribes to surface messages.
        /// </summary>
        public void Initialize()
        {
            for (int i = 0; i < _data.Windows.Length; i++)
            {
                SampleWindow window = _data.Windows[i];
                window.Root.SetActive(_surfaces.IsOpen(window.Inventory.Surface));
            }

            _subscriptions = DisposableBag.Create(
                _openedSubscriber.Subscribe(OnOpened),
                _closedSubscriber.Subscribe(OnClosed));
        }

        /// <summary>
        /// Unsubscribes.
        /// </summary>
        public void Dispose() => _subscriptions.Dispose();

        private void OnOpened(SurfaceOpenedMessage message) => SetVisible(message.Surface, true);

        private void OnClosed(SurfaceClosedMessage message) => SetVisible(message.Surface, false);

        private void SetVisible(GridSurface surface, bool isVisible)
        {
            for (int i = 0; i < _data.Windows.Length; i++)
            {
                if (_data.Windows[i].Inventory.Surface == surface)
                {
                    _data.Windows[i].Root.SetActive(isVisible);
                }
            }
        }
    }
}
