namespace GridSystem.DragDropSystem.View
{
    using IdentificationSystem;
    using OverlappedSystem;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;
#if DOTWEEN
    using DG.Tweening;
    using UnityEngine.InputSystem;
#endif

    public class UIDraggableObject : BasePoolableDraggableObject
    {
        [Inject]
        private void Construct(IIdentificationSystem identificationSystem, UIOverlapSystem overlapSystem)
        {
            this.identificationSystem = identificationSystem;
            this.overlapSystem = overlapSystem;
        }

        [field: SerializeField] public Image Image { get; private set; }
        [field: SerializeField] protected Image RayImage { get; set; }

        #region UNITY_REGION

        private void Start() => RayImage.alphaHitTestMinimumThreshold = 0.1f;

        #endregion

        #region PUBLIC_REGION

        public override void ApplyRotation(int rotationCount)
        {
            this.rotationCount = rotationCount;
            rotationAngle = -rotationAngleOffset * this.rotationCount;
            if (Transform is RectTransform rectTransform)
            {
                rectTransform.eulerAngles = new Vector3(0, 0, rotationAngle);
            }
        }

        #endregion

        #region PRIVATE_REGION

        protected override void ConfigureBody()
        {
            if (Transform is RectTransform rectTransform && ItemData.UIPrefab.Transform is RectTransform prefabRect)
            {
                rectTransform.anchoredPosition = prefabRect.anchoredPosition;
                rectTransform.pivot = prefabRect.pivot;
                rectTransform.sizeDelta = prefabRect.sizeDelta;
            }

            Image.sprite = ItemData.UIPrefab.Image.sprite;
            RayImage.sprite = Image.sprite;

            gameObject.name = $"{ItemData.UIPrefab.gameObject.name}_{ID}";
        }

#if DOTWEEN
        protected override void RotateBody()
        {
            Transform.DORotate(new Vector3(0, 0, rotationAngle), 0.1f);
            Transform.DOMove(Mouse.current.position.ReadValue(), 0.1f);
        }
#else
        protected override void RotateBody() => Transform.localEulerAngles = new Vector3(0, 0, rotationAngle);
#endif

        #endregion
    }
}