namespace GridSystem.DestroySystem
{
    using Signals;
    using UnityEngine;
    using Zenject;

    public sealed class DestroySystem : IInitializable
    {
        public DestroySystem(SignalBus signalBus) => _signalBus = signalBus;

        private readonly SignalBus _signalBus;
        public void Initialize() => _signalBus.Subscribe<ItemMergedSignal>(InitializeDestroyMergeItems);

        private void InitializeDestroyMergeItems(ItemMergedSignal signal)
        {
            Object.Destroy(signal.FirstItem.Transform.gameObject);
            Object.Destroy(signal.SecondItem.Transform.gameObject);
        }
    }
}