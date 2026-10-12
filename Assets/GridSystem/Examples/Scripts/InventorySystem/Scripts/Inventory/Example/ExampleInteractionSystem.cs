namespace GridSystem.InventorySystem.Inventory.Example
{
    using Core;
    using Enums;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Zenject;

    public sealed class ExampleInteractionSystem : MonoBehaviour
    {
        [Inject]
        private void Construct(IInventoryManager inventoryManager) => _inventoryManager = inventoryManager;

        private IInventoryManager _inventoryManager;

        [SerializeField]
        private InventoryType _inventoryType;

        [SerializeField]
        private InputActionReference _actionReference;

        [SerializeField]
        private bool _isOpenAfterCreating;

        private bool _isOpen;
        private InventoryIdentificationData _inventoryIdentificationData;

        private void Start()
        {
            if (_inventoryManager.CreateInventory(_inventoryType, out int inventoryID))
            {
                _inventoryIdentificationData = new InventoryIdentificationData
                {
                    ID = inventoryID,
                    InventoryType = _inventoryType
                };
                if (_actionReference != null)
                {
                    _actionReference.action.performed += DoAfterInput;
                    _actionReference.action.Enable();
                }
            }

            if (_isOpenAfterCreating)
            {
                _inventoryManager.OpenInventoryWithID(_inventoryIdentificationData);
                _isOpen = !_isOpen;
            }
        }

        private void OnDestroy()
        {
            if (_actionReference != null)
            {
                _actionReference.action.performed -= DoAfterInput;
                _actionReference.action.Disable();
            }
        }

        private void DoAfterInput(InputAction.CallbackContext obj)
        {
            if (!_isOpen)
            {
                _inventoryManager.OpenInventoryWithID(_inventoryIdentificationData);
            }
            else
            {
                _inventoryManager.CloseInventory(_inventoryIdentificationData);
            }

            _isOpen = !_isOpen;
        }
    }
}