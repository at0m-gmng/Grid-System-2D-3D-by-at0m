namespace GridSystem.Runtime.Pointer
{
    using System;
    using Composition;
    using MessagePipe;
    using Messaging;
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Translates Unity Input System actions into pointer messages. Replace it to use another input source.
    /// </summary>
    public sealed class InputSystemPointerInput : IGridInitializable, IDisposable
    {
        public InputSystemPointerInput(PointerInputConfig config, IPublisher<PointerMovedMessage> movedPublisher,
            IPublisher<PointerButtonMessage> buttonPublisher, IPublisher<RotateRequestedMessage> rotatePublisher)
        {
            _config = config;
            _movedPublisher = movedPublisher;
            _buttonPublisher = buttonPublisher;
            _rotatePublisher = rotatePublisher;
            _onPoint = OnPoint;
            _onPressStarted = OnPressStarted;
            _onPressCanceled = OnPressCanceled;
            _onRotatePerformed = OnRotatePerformed;
        }

        private readonly PointerInputConfig _config;
        private readonly IPublisher<PointerMovedMessage> _movedPublisher;
        private readonly IPublisher<PointerButtonMessage> _buttonPublisher;
        private readonly IPublisher<RotateRequestedMessage> _rotatePublisher;
        private readonly Action<InputAction.CallbackContext> _onPoint;
        private readonly Action<InputAction.CallbackContext> _onPressStarted;
        private readonly Action<InputAction.CallbackContext> _onPressCanceled;
        private readonly Action<InputAction.CallbackContext> _onRotatePerformed;
        private Vector2 _position;

        /// <summary>
        /// Subscribes and enables actions.
        /// </summary>
        public void Initialize()
        {
            _config.Point.performed += _onPoint;
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
            _config.Point.performed -= _onPoint;
            _config.Press.started -= _onPressStarted;
            _config.Press.canceled -= _onPressCanceled;
            _config.Rotate.performed -= _onRotatePerformed;
            _config.Point.Disable();
            _config.Press.Disable();
            _config.Rotate.Disable();
        }

        private void OnPoint(InputAction.CallbackContext context)
        {
            _position = context.ReadValue<Vector2>();
            _movedPublisher.Publish(new PointerMovedMessage(_position));
        }

        private void OnPressStarted(InputAction.CallbackContext context) => _buttonPublisher.Publish(new PointerButtonMessage(_position, true));

        private void OnPressCanceled(InputAction.CallbackContext context) => _buttonPublisher.Publish(new PointerButtonMessage(_position, false));

        private void OnRotatePerformed(InputAction.CallbackContext context) => _rotatePublisher.Publish(new RotateRequestedMessage());
    }
}
