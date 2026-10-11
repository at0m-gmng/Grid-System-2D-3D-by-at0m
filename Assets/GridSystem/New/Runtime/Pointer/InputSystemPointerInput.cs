namespace GridSystem.Runtime.Pointer
{
    using System;
    using Composition;
    using R3;
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Pointer source based on Unity Input System actions.
    /// </summary>
    public sealed class InputSystemPointerInput : IPointerInput, IGridInitializable, IDisposable
    {
        public InputSystemPointerInput(PointerInputConfig config)
        {
            _config = config;
            _onPressStarted = OnPressStarted;
            _onPressCanceled = OnPressCanceled;
            _onRotatePerformed = OnRotatePerformed;
        }

        private readonly PointerInputConfig _config;
        private readonly Action<InputAction.CallbackContext> _onPressStarted;
        private readonly Action<InputAction.CallbackContext> _onPressCanceled;
        private readonly Action<InputAction.CallbackContext> _onRotatePerformed;
        private readonly Subject<Unit> _pressed = new();
        private readonly Subject<Unit> _released = new();
        private readonly Subject<Unit> _rotated = new();

        /// <summary>
        /// Current screen position.
        /// </summary>
        public Vector2 Position => _config.Point.ReadValue<Vector2>();

        /// <summary>
        /// Raised on press.
        /// </summary>
        public Observable<Unit> Pressed => _pressed;

        /// <summary>
        /// Raised on release.
        /// </summary>
        public Observable<Unit> Released => _released;

        /// <summary>
        /// Raised on rotate request.
        /// </summary>
        public Observable<Unit> Rotated => _rotated;

        /// <summary>
        /// Subscribes and enables actions.
        /// </summary>
        public void Initialize()
        {
            _config.Press.started += _onPressStarted;
            _config.Press.canceled += _onPressCanceled;
            _config.Rotate.performed += _onRotatePerformed;
            _config.Point.Enable();
            _config.Press.Enable();
            _config.Rotate.Enable();
        }

        /// <summary>
        /// Unsubscribes and disables actions.
        /// </summary>
        public void Dispose()
        {
            _config.Press.started -= _onPressStarted;
            _config.Press.canceled -= _onPressCanceled;
            _config.Rotate.performed -= _onRotatePerformed;
            _config.Point.Disable();
            _config.Press.Disable();
            _config.Rotate.Disable();
            _pressed.Dispose();
            _released.Dispose();
            _rotated.Dispose();
        }

        private void OnPressStarted(InputAction.CallbackContext context) => _pressed.OnNext(Unit.Default);

        private void OnPressCanceled(InputAction.CallbackContext context) => _released.OnNext(Unit.Default);

        private void OnRotatePerformed(InputAction.CallbackContext context) => _rotated.OnNext(Unit.Default);
    }
}
