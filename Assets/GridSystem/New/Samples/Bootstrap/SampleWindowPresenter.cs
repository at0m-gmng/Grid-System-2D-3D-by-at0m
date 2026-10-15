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
        public SampleWindowPresenter(SampleContent content, ISurfaceService surfaces,
            ISubscriber<SurfaceOpenedMessage> openedSubscriber, ISubscriber<SurfaceClosedMessage> closedSubscriber)
        {
            _content = content;
            _surfaces = surfaces;
            _openedSubscriber = openedSubscriber;
            _closedSubscriber = closedSubscriber;
        }

        private readonly SampleContent _content;
        private readonly ISurfaceService _surfaces;
        private readonly ISubscriber<SurfaceOpenedMessage> _openedSubscriber;
        private readonly ISubscriber<SurfaceClosedMessage> _closedSubscriber;
        private IDisposable _subscriptions;

        /// <summary>
        /// Syncs window states and subscribes to surface messages.
        /// </summary>
        public void Initialize()
        {
            _content.Ui.MainWindow.SetActive(_surfaces.IsOpen(_content.Ui.Main));
            _content.Ui.ChestWindow.SetActive(_surfaces.IsOpen(_content.Ui.Chest));
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
            if (message.Surface == _content.Ui.Main)
            {
                _content.Ui.MainWindow.SetActive(true);
            }
            else if (message.Surface == _content.Ui.Chest)
            {
                _content.Ui.ChestWindow.SetActive(true);
            }
        }

        private void OnClosed(SurfaceClosedMessage message)
        {
            if (message.Surface == _content.Ui.Main)
            {
                _content.Ui.MainWindow.SetActive(false);
            }
            else if (message.Surface == _content.Ui.Chest)
            {
                _content.Ui.ChestWindow.SetActive(false);
            }
        }
    }
}
