namespace GridSystem.DragDropSystem
{
    using System;
    using InputSystem;
    using OverlappedSystem;
    using Signals;
    using UniRx;
    using UnityEngine;
    using View;
    using Zenject;

    public abstract class AbstractDragDropSystem : IDisposable
    {
        public AbstractDragDropSystem(IInputSystem inputEvents, AbstractOverlapSystem overlapSystem, SignalBus signalBus)
        {
            _inputEvents = inputEvents;

            _inputEvents.BeginDrag.Subscribe(_ => StartDrag()).AddTo(_compositeDisposable);
            _inputEvents.Drag.Subscribe(_ => Drag()).AddTo(_compositeDisposable);
            _inputEvents.EndDrag.Subscribe(_ => EndDrag()).AddTo(_compositeDisposable);
            _inputEvents.Rotate.Subscribe(_ => Rotate()).AddTo(_compositeDisposable);
            _signalBus = signalBus;

            inputEvents.Enable();
        }

        protected IInputSystem _inputEvents;
        protected AbstractOverlapSystem _overlapSystem;
        protected SignalBus _signalBus;

        protected virtual Vector2 MousePosition => _inputEvents.MousePosition;

        protected virtual IDraggableObject DraggableObject { get; set; }

        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
            _compositeDisposable = null;
            _inputEvents.Disable();
        }

        protected virtual void StartDrag()
        {
            if (_overlapSystem.TryGetOverlappedItem(_inputEvents.MousePosition, out BaseDraggableObject result))
            {
                DraggableObject = result;
                DraggableObject?.StartDrag();
                _signalBus.Fire(new DragDropStateSignal
                {
                    IsDrag = true
                });
            }
        }

        protected virtual void Drag()
        {
            if (DraggableObject != null)
            {
                DraggableObject?.Drag(MousePosition);
            }
        }

        protected virtual void EndDrag()
        {
            if (DraggableObject != null)
            {
                DraggableObject?.EndDrag();
                DraggableObject = null;
                _signalBus.Fire(new DragDropStateSignal
                {
                    IsDrag = false
                });
            }
        }

        protected virtual void Rotate()
        {
            if (DraggableObject != null)
            {
                DraggableObject?.Rotate();
            }
        }
    }
}