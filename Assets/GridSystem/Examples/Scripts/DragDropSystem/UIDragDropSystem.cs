namespace GridSystem.DragDropSystem
{
    using InputSystem;
    using OverlappedSystem;
    using Zenject;

    public sealed class UIDragDropSystem : AbstractDragDropSystem
    {
        public UIDragDropSystem(IInputSystem inputEvents, UIOverlapSystem overlapSystem, SignalBus signalBus)
            : base(inputEvents, overlapSystem, signalBus)
        {
            _inputEvents = inputEvents;
            _overlapSystem = overlapSystem;
            _signalBus = signalBus;
        }
    }
}