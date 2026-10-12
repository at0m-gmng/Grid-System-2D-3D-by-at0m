namespace GridSystem.DragDropSystem.View
{
    using System;
    using IdentificationSystem;
    using OverlappedSystem;
    using UnityEngine;
    using Zenject;
#if DOTWEEN
    using DG.Tweening;
#endif

    public class WorldDraggableObject : BasePoolableDraggableObject
    {
        [Inject]
        private void Construct(IIdentificationSystem identificationSystem, WorldOverlapSystem overlapSystem)
        {
            this.identificationSystem = identificationSystem;
            this.overlapSystem = overlapSystem;
        }

        protected GameObject body;

        #region PUBLIC_REGION

        public override void ApplyRotation(int rotationCount)
        {
            this.rotationCount = rotationCount;
            rotationAngle = -rotationAngleOffset * this.rotationCount;
            transform.eulerAngles = new Vector3(0, rotationAngle, 0);
        }

        #endregion

        #region DRAG&DROP_REGION

        public override void Drag(Vector2 position)
        {
            this.position = position;
            transform.position = new Vector3(position.x, transform.position.y, position.y);
            CheckAvaillablePlaced();
        }

        #endregion

        #region POOL_REGION

        public override void OnDespawned()
        {
            identificationSystem.UnRegistry(ID);
            transform.SetParent(poolParent);
            pool = null;
            Destroy(body);
        }

        #endregion

        #region PRIVATE_REGION

        protected override void ConfigureBody()
        {
            body = Instantiate(ItemData.WorldPrefab, objectTransform);
            body.transform.localPosition = Vector3.zero;
            body.transform.localRotation = default;

            gameObject.name = $"{ItemData.WorldPrefab.gameObject.name}_{ID}";

            if (Facade == null)
            {
                throw new NotImplementedException();
            }
        }

        private Sequence _sequence;

#if DOTWEEN
        protected override void RotateBody()
        {
            transform.DOKill(true);
            transform.DOMove(Facade.Pivot.position + Quaternion.AngleAxis(-rotationAngleOffset, Vector3.up) * (transform.position - Facade.Pivot.position), 0.3f);
            transform.DORotateQuaternion(transform.rotation * Quaternion.AngleAxis(-rotationAngleOffset, Vector3.up), 0.3f);
        }
#else
        protected override void RotateBody() 
            => transform.RotateAround(Facade.Pivot.transform.position, Vector3.up, -rotationAngleOffset);
#endif

        #endregion
    }
}