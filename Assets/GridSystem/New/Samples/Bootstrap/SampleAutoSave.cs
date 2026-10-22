namespace GridSystem.Samples.Bootstrap
{
    using System;
    using MessagePipe;
    using Runtime.Composition;
    using Runtime.Messaging;
    using Runtime.Persistence;

    /// <summary>
    /// Saves an inventory when its surface is closed.
    /// </summary>
    public sealed class SampleAutoSave : IGridInitializable, IDisposable
    {
        public SampleAutoSave(IInventoryPersistence persistence, ISubscriber<SurfaceClosedMessage> closedSubscriber)
        {
            _persistence = persistence;
            _closedSubscriber = closedSubscriber;
        }

        private readonly IInventoryPersistence _persistence;
        private readonly ISubscriber<SurfaceClosedMessage> _closedSubscriber;
        private IDisposable _subscription;

        /// <summary>
        /// Subscribes to surface close.
        /// </summary>
        public void Initialize() => _subscription = _closedSubscriber.Subscribe(OnClosed);

        /// <summary>
        /// Unsubscribes.
        /// </summary>
        public void Dispose() => _subscription?.Dispose();

        private void OnClosed(SurfaceClosedMessage message) => _persistence.Save(message.Inventory);
    }
}
