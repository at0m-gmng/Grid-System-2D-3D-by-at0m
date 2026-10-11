namespace GridSystem.Samples.Bootstrap
{
    using System;
    using Core.Inventories;
    using Runtime.Surfaces;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Opens and closes the window inventory on its surface by the toggle action.
    /// </summary>
    public sealed class SampleWindowController
    {
        public SampleWindowController(SampleWindow window, GridInventory inventory, ISurfaceService surfaces)
        {
            _window = window;
            _surfaces = surfaces;
            Inventory = inventory;
            _onToggle = OnToggle;
        }

        private readonly SampleWindow _window;
        private readonly ISurfaceService _surfaces;
        private readonly Action<InputAction.CallbackContext> _onToggle;

        /// <summary>
        /// Window inventory.
        /// </summary>
        public GridInventory Inventory { get; }

        /// <summary>
        /// Applies the start state and listens to the toggle action.
        /// </summary>
        public void Bind()
        {
            SetOpened(_window.IsOpenedOnStart);
            _window.Toggle.performed += _onToggle;
            _window.Toggle.Enable();
        }

        /// <summary>
        /// Stops listening to the toggle action.
        /// </summary>
        public void Unbind() => _window.Toggle.performed -= _onToggle;

        private void OnToggle(InputAction.CallbackContext context) => SetOpened(!_surfaces.IsOpen(_window.Inventory.Surface));

        private void SetOpened(bool isOpened)
        {
            if (isOpened)
            {
                _surfaces.Open(_window.Inventory.Surface, Inventory);
            }
            else
            {
                _surfaces.Close(_window.Inventory.Surface);
            }
        }
    }
}
