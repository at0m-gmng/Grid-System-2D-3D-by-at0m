namespace GridSystem.InventorySystem.Views
{
    using System;
    using System.Collections.Generic;
    using Inventory.Core;
    using Inventory.Enums;
    using Signals;
    using UnityEngine;
    using Zenject;

    public sealed class InventoriesView : MonoBehaviour
    {
        [Inject]
        private void Construct(IInventoryManager inventoryManager, SignalBus signalBus)
        {
            _signalBus = signalBus;
            _signalBus.Subscribe<OpenInventorySignal>(InitializeInventory);
        }

        private SignalBus _signalBus;

        [SerializeField]
        private List<InventoryDataView> _targetInventories = new();

        private void InitializeInventory(OpenInventorySignal signal)
        {
            InventoryDataView inventoryDataView = _targetInventories.Find(x => x.InventoryType.Contains(signal.InventorySystemData.InventoryType));
            if (inventoryDataView != null)
            {
                inventoryDataView.TargetObject.SetActive(signal.IsOpen);
            }
        }
    }

    [Serializable]
    public class InventoryDataView
    {
        [field: SerializeField] public List<InventoryType> InventoryType { get; private set; }
        [field: SerializeField] public GameObject TargetObject { get; private set; }
    }
}