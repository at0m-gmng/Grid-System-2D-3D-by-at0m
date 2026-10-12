namespace GridSystem.DestroySystem.PlacementSystem
{
    using System;
    using System.Collections.Generic;
    using GridView;
    using InventoryPresenters;
    using InventorySystem.Inventory.Enums;
    using Signals;
    using UnityEngine;
    using Zenject;

    public sealed class PlacementSystem : MonoBehaviour
    {
        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
            _signalBus.Subscribe<CreatedItemSignal>(InitializePlacement);
        }

        private SignalBus _signalBus;

        [SerializeField]
        private List<UIGridView> _inventoryPlacements = new();

        private void InitializePlacement(CreatedItemSignal signal)
        {
            UIGridView inventoryPlacement = _inventoryPlacements.Find(x => x.InventoryPresenter == signal.UIDraggableObject.InventoryView);

            if (inventoryPlacement != null)
            {
                inventoryPlacement.InventoryPresenter.TryAutoPlaceItem(signal.UIDraggableObject);
            }
        }
    }

    [Serializable]
    public class InventoryPlacement
    {
        [field: SerializeField] public UIInventoryPresenter InventoryView { get; private set; } = default;
        [field: SerializeField] public List<InventoryType> InventoryType { get; private set; }
    }
}