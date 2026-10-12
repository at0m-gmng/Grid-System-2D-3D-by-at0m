namespace GridSystem.DragDropSystem.View
{
    using Facade;
    using UnityEngine;

    public abstract class BaseDraggableObject : MonoBehaviour, IDraggableObject
    {
        protected DraggableItemFacade Facade
        {
            get
            {
                if (_facade == null)
                {
                    _facade = GetComponentInChildren<DraggableItemFacade>();
                }

                return _facade;
            }
        }

        [SerializeField]
        protected Transform objectTransform;

        [SerializeField]
        protected float rotationAngleOffset = 90f;

        [SerializeField]
        private DraggableItemFacade _facade;

        protected Vector2 position;
        protected int rotationCount;
        protected int startRotationCount = 0;
        protected float rotationAngle;

        public abstract void StartDrag();
        public abstract void Drag(Vector2 position);

        public abstract void EndDrag();

        public virtual void Rotate()
        {
            rotationCount = (rotationCount + 1) & 3;
            rotationAngle -= rotationAngleOffset;
            UpdateRotationView();
        }

        protected abstract void UpdateRotationView();
    }

    public interface IDraggableObject
    {
        public void StartDrag();
        public void Drag(Vector2 position);
        public void EndDrag();
        public void Rotate();
    }
}