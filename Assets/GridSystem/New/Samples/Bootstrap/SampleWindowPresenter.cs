namespace GridSystem.Samples.Bootstrap
{
    using System;
    using MessagePipe;
    using Runtime.Composition;
    using Runtime.Messaging;
    using Runtime.Surfaces;

    /// <summary>
    /// Shows UI windows while their surfaces are open. Handles main and chest windows.
    /// </summary>
    public sealed class SampleWindowPresenter : IGridInitializable, IDisposable
    {
        public SampleWindowPresenter(SampleSceneData data, ISurfaceService surfaces,
            ISubscriber<SurfaceOpenedMessage> openedSubscriber, ISubscriber<SurfaceClosedMessage> closedSubscriber)
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
        /// Syncs window states and subscribes to surface messages.
        /// </summary>
        public void Initialize()
        {
            _data.MainWindow.SetActive(_surfaces.IsOpen(_data.Main.Surface));
            _data.ChestWindow.SetActive(_surfaces.IsOpen(_data.Chest.Surface));
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
            else if (message.Surface == _data.Chest.Surface)
            {
                _data.ChestWindow.SetActive(true);
            }
        }

        private void OnClosed(SurfaceClosedMessage message)
        {
            if (message.Surface == _data.Main.Surface)
            {
                _data.MainWindow.SetActive(false);
            }
            else if (message.Surface == _data.Chest.Surface)
            {
                _data.ChestWindow.SetActive(false);
            }
        }
    }
}
