namespace GridSystem.DescriptionSystem
{
    using DragDropSystem.View;
    using Signals;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using Zenject;

    public class AbstractDescriptionAdapter<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        where T : IPoolableDraggableObject
    {
        [Inject]
        private void Construct(SignalBus signalBus) => _signalBus = signalBus;

        private SignalBus _signalBus;

        [SerializeField]
        protected T draggableObject;

        public void OnPointerEnter(PointerEventData eventData)
        {
            _signalBus.Fire(new ItemDataViewSignal
            {
                ItemDataView = draggableObject.ItemDataView,
                ItemTransform = transform
            });
        }

        public void OnPointerExit(PointerEventData eventData) => _signalBus.Fire(new ItemDataViewSignal());
    }
}