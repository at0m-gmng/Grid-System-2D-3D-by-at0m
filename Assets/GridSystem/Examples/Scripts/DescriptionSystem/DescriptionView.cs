namespace GridSystem.DescriptionSystem
{
    using Signals;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public sealed class DescriptionView : MonoBehaviour
    {
        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
            _signalBus.Subscribe<ItemDataViewSignal>(OnItemDataViewShowed);
            _signalBus.Subscribe<DragDropStateSignal>(OnDragDropStateSignal);
        }

        private SignalBus _signalBus;

        [Header("PopUp")]
        [field: SerializeField]
        public GameObject ItemPopUp { get; private set; }

        [field: SerializeField] public RectTransform PopUpRect { get; private set; }

        [Header("Params")]
        [field: SerializeField]
        public Text NameValue { get; private set; }

        [field: SerializeField] public Text DamageValue { get; private set; }
        [field: SerializeField] public Text AtackSpeedValue { get; private set; }
        [field: SerializeField] public Text RangeValue { get; private set; }

        private bool _isLockShowedPopUp;
        private Transform _lastTransform;

        private void OnItemDataViewShowed(ItemDataViewSignal signal)
        {
            ItemPopUp.gameObject.SetActive(signal.ItemDataView != null && !_isLockShowedPopUp);

            if (signal.ItemDataView != null)
            {
                NameValue.text = signal.ItemDataView.Name;
                DamageValue.text = signal.ItemDataView.Damage.ToString();
            }
        }

        private void OnDragDropStateSignal(DragDropStateSignal signal)
        {
            _isLockShowedPopUp = signal.IsDrag;
            ItemPopUp.gameObject.SetActive(!_isLockShowedPopUp);
        }
    }
}