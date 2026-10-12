namespace GridSystem.InventorySystem.Inventory.Example
{
    using Configs;
    using InputSystem;
    using Signals;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public sealed class ExampleSelectableSpawnSystem : MonoBehaviour
    {
        [Inject]
        private void Construct(SignalBus signalBus, SelectableItemConfig selectableItemConfig, IInputSystem inputEvents)
        {
            _selectableItemConfig = selectableItemConfig;
            _signalBus = signalBus;
            _inputEvents = inputEvents;
            _signalBus.Subscribe<SelectedItemSignal>(InitializePlacement);
            _inputEvents.EndDrag.Subscribe(_ => UpdateLayout()).AddTo(_compositeDisposable);
        }

        private SelectableItemConfig _selectableItemConfig;
        private SignalBus _signalBus;
        private IInputSystem _inputEvents;

        [SerializeField]
        private Transform _selectableSpawnTransform;

        [SerializeField]
        private LayoutGroup _layoutGroup;

        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private void Start()
        {
            _signalBus.Fire(new DataSelectedForSpawnSignal
            {
                SelectedForSpawn = _selectableItemConfig.GetRandomItemsForLevel(1)
            });
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
            _compositeDisposable = null;
        }

        private void InitializePlacement(SelectedItemSignal signal)
            => signal.UIDraggableObject.Transform.SetParent(_selectableSpawnTransform);

        private void UpdateLayout()
        {
            _layoutGroup.CalculateLayoutInputHorizontal();
            _layoutGroup.SetLayoutHorizontal();
            _layoutGroup.CalculateLayoutInputVertical();
            _layoutGroup.SetLayoutVertical();
        }
    }
}