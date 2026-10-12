namespace GridSystem.DragDropSystem
{
    using InputSystem;
    using OverlappedSystem;
    using Signals;
    using UnityEngine;
    using View;
    using Zenject;

    public sealed class WorldDragDropSystem : AbstractDragDropSystem
    {
        public WorldDragDropSystem(IInputSystem inputEvents, WorldOverlapSystem overlapSystem, SignalBus signalBus)
            : base(inputEvents, overlapSystem, signalBus)
        {
            _inputEvents = inputEvents;
            _overlapSystem = overlapSystem;
            _signalBus = signalBus;
            _camera = Camera.main;
        }

        protected Camera _camera;
        protected Ray _rayOrigin;
        protected Vector3 _position;
        protected float _dragPlaneY;
        protected float _invDirectionY;
        protected float _direction;

        protected override Vector2 MousePosition
        {
            get
            {
                _rayOrigin = _camera.ScreenPointToRay(_inputEvents.MousePosition);

                _invDirectionY = 1f / _rayOrigin.direction.y;
                _direction = (_dragPlaneY - _rayOrigin.origin.y) * _invDirectionY;
                _position = _rayOrigin.origin + _rayOrigin.direction * _direction;
                return new Vector2(_position.x, _position.z);
            }
        }

        protected override void StartDrag()
        {
            if (_overlapSystem.TryGetOverlappedItemInParent(_inputEvents.MousePosition, out BaseDraggableObject result))
            {
                _dragPlaneY = result.transform.position.y;
                DraggableObject = result;
                DraggableObject?.StartDrag();
                _signalBus.Fire(new DragDropStateSignal
                {
                    IsDrag = true
                });
            }
        }
    }
}