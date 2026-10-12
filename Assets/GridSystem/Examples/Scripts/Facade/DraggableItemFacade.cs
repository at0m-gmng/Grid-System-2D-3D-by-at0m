namespace GridSystem.Facade
{
    using ColorizeSystem;
    using UnityEngine;

    public class DraggableItemFacade : MonoBehaviour
    {
        public AbstractColorizeProvider ColorizeProvider
        {
            get
            {
                if (_colorizeProvider == null)
                {
                    _colorizeProvider = GetComponentInChildren<AbstractColorizeProvider>();
                }

                return _colorizeProvider;
            }
        }

        public Transform Pivot
        {
            get
            {
                if (_pivot == null)
                {
                    _pivot = transform.GetChild(transform.childCount - 1);
                }

                return _pivot;
            }
        }

        [SerializeField]
        private AbstractColorizeProvider _colorizeProvider;

        [SerializeField]
        private Transform _pivot;
    }
}