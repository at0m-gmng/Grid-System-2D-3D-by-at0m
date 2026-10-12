namespace GridSystem.Samples.Bootstrap
{
    using System;
    using MessagePipe;
    using Runtime.Composition;
    using Runtime.Messaging;
    using Runtime.Surfaces;

    /// <summary>
    /// Shows the main window while its surface is opened.
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
        /// Syncs window state and subscribes to surface messages.
        /// </summary>
        public void Initialize()
        {
            _data.MainWindow.SetActive(_surfaces.IsOpen(_data.Main.Surface));
            _subscriptions = DisposableBag.Create(
                _openedSubscriber.Subscribe(OnOpened),
                _closedSubscriber.Subscribe(OnClosed));
        }

        /// <summary>
        /// Unsubscribes.
        /// </summary>
        public void Dispose() => _subscriptions.Dispose();

        private void OnOpened(SurfaceOpenedMessage message)
        {
            if (message.Surface == _data.Main.Surface)
            {
                _data.MainWindow.SetActive(true);
            }
        }

        private void OnClosed(SurfaceClosedMessage message)
        {
            if (message.Surface == _data.Main.Surface)
            {
                _data.MainWindow.SetActive(false);
            }
        }
    }
}
