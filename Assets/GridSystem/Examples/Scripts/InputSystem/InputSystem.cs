namespace InputSystem
{
    using System;
    using UniRx;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public sealed class InputSystem : IInputSystem
    {
        public InputSystem(InputConfig config)
        {
            _beginDragAction = config.BeginDrag.action;
            _dragAction = config.Drag.action;
            _endDragAction = config.EndDrag.action;
            _rotateAction = config.Rotation.action;

            if (_beginDragAction == null || _dragAction == null || _endDragAction == null || _rotateAction == null)
            {
#if UNITY_EDITOR
                Debug.LogError("InputActionReference in config is null!");
#endif
                return;
            }

            _beginDragAction.started += _ =>
            {
                _isDragging = true;
                _beginDrag.Execute();
            };

            _dragAction.performed += ctx =>
            {
                if (_isDragging)
                {
                    _drag.Execute();
                }
            };

            _endDragAction.canceled += _ =>
            {
                _isDragging = false;
                _endDrag.Execute();
            };

            _rotateAction.performed += ctx =>
            {
                if (_isDragging)
                {
                    _rotate.Execute();
                }
            };
        }

        private readonly InputAction _beginDragAction;
        private readonly InputAction _dragAction;
        private readonly InputAction _endDragAction;
        private readonly InputAction _rotateAction;

        public IObservable<Unit> BeginDrag => _beginDrag;
        public IObservable<Unit> Drag => _drag;
        public IObservable<Unit> EndDrag => _endDrag;
        public IObservable<Unit> Rotate => _rotate;

        public Vector2 MousePosition => Mouse.current.position.ReadValue();

        private readonly ReactiveCommand _beginDrag = new();
        private readonly ReactiveCommand _drag = new();
        private readonly ReactiveCommand _endDrag = new();
        private readonly ReactiveCommand _rotate = new();
        private bool _isDragging;

        public void Enable()
        {
            _beginDragAction?.Enable();
            _dragAction?.Enable();
            _endDragAction?.Enable();
            _rotateAction?.Enable();
        }

        public void Disable()
        {
            _beginDragAction?.Disable();
            _dragAction?.Disable();
            _endDragAction?.Disable();
            _rotateAction?.Disable();

            _beginDragAction.started -= _ => _beginDrag.Execute();
            _dragAction.performed -= _ => _drag.Execute();
            _endDragAction.canceled -= _ => _endDrag.Execute();
            _rotateAction.performed -= _ => _rotate.Execute();
        }
    }

    public interface IInputEvents
    {
        public Vector2 MousePosition { get; }
        public IObservable<Unit> BeginDrag { get; }
        public IObservable<Unit> Drag { get; }
        public IObservable<Unit> EndDrag { get; }
        public IObservable<Unit> Rotate { get; }
    }

    public interface IInputSystem : IInputEvents
    {
        public void Enable();
        public void Disable();
    }
}