namespace GridSystem.Samples.Bootstrap
{
    using System;
    using MessagePipe;
    using Runtime.Composition;
    using Runtime.Messaging;
    using Runtime.Persistence;

    /// <summary>
    /// Saves a persistent inventory when its surface is closed. Transient ones are skipped,
    /// so a bar refilled on every launch leaves no stale save behind.
    /// </summary>
    public sealed class SampleAutoSave : IGridInitializable, IDisposable
    {
        public SampleAutoSave(SampleSceneConfig config, IInventoryPersistence persistence, ISubscriber<SurfaceClosedMessage> closedSubscriber)
        {
            _config = config;
            _persistence = persistence;
            _closedSubscriber = closedSubscriber;
        }

        private readonly SampleSceneConfig _config;
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

        private void OnClosed(SurfaceClosedMessage message)
        {
            if (_config.IsPersistent(message.Inventory.Key))
            {
                _persistence.Save(message.Inventory);
            }
        }
    }
}
