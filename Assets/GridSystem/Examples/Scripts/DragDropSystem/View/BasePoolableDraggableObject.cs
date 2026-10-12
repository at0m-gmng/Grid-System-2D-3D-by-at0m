namespace GridSystem.DragDropSystem.View
{
    using Factories;
    using GridView;
    using IdentificationSystem;
    using InventoryPresenters;
    using InventorySystem.Inventory.Data;
    using OverlappedSystem;
    using UnityEngine;
    using Zenject;

    public abstract class BasePoolableDraggableObject : BaseDraggableObject, IPoolableDraggableObject,
        IPoolable<DraggableData, IMemoryPool>
    {
        protected IIdentificationSystem identificationSystem;
        protected AbstractOverlapSystem overlapSystem;

        public Transform Transform => objectTransform;

        public IInventoryPresenter InventoryView { get; private set; }

        public virtual BaseItem ItemData { get; set; }

        public virtual BaseItemView ItemDataView { get; protected set; }

        public int ID { get; private set; }

        public PlacementItem PlacementItem
        {
            get => placementItem;
            set => placementItem = value;
        }

        protected IMemoryPool pool;
        protected Transform poolParent;

        protected Vector3 startPosition;
        protected Transform startParent;
        protected Transform tempParent;
        protected BaseItem tempData;
        protected PlacementItem placementItem;

        #region PUBLIC_REGION

        public abstract void ApplyRotation(int rotationCount);

        #endregion

        #region DRAG&DROP_REGION

        public override void StartDrag()
        {
            startPosition = transform.position;
            startParent = transform.parent;
            tempData = ItemData;
            startRotationCount = rotationCount;
            if (InventoryView != null)
            {
                InventoryView.TryReleasePlacement(placementItem);
            }

            transform.SetParent(tempParent);
            transform.SetAsLastSibling();

            ColorizeItem(InventoryView);
        }

        public override void Drag(Vector2 position)
        {
            this.position = position;
            Transform.position = position;
            CheckAvaillablePlaced();
        }

        public override void EndDrag()
        {
            ResetPosition();
            Facade.ColorizeProvider.ColorizeToBase();
            tempData = default;
        }

        #endregion

        #region POOL_REGION

        public void OnSpawned(DraggableData uiDraggableData, IMemoryPool pool)
        {
            this.pool = pool;
            poolParent = transform.parent;
            InventoryView = uiDraggableData.InventoryView;
            ItemData = uiDraggableData.ItemData.Item;
            ItemDataView = uiDraggableData.ItemData.BaseItemView;
            tempParent = uiDraggableData.TempParent;
            if (identificationSystem.Registry(out int id))
            {
                ID = id;
            }

            placementItem = new PlacementItem
            {
                ID = ItemData.Type,
                PlacementCenter = uiDraggableData.PlacementItem?.PlacementCenter ?? default,
                PlacementCells = uiDraggableData.PlacementItem?.PlacementCells,
                Shape = uiDraggableData.PlacementItem?.Shape ?? ItemData.TryGetItemSize(),
                RotationIndex = uiDraggableData.PlacementItem?.RotationIndex ?? 0
            };
            ItemData = ItemData.GetRotation(placementItem.RotationIndex);
            ConfigureItem();
        }

        public virtual void OnDespawned()
        {
            identificationSystem.UnRegistry(ID);
            transform.SetParent(poolParent);
            pool = null;
        }

        public virtual void ReturnToPool()
        {
            if (pool != null)
            {
                pool.Despawn(this);
            }
        }

        #endregion

        #region PRIVATE_REGION

        private void ConfigureItem()
        {
            ConfigureBody();
            ApplyRotation(placementItem.RotationIndex);
        }

        protected virtual void CheckAvaillablePlaced()
        {
            if (overlapSystem.TryGetOverlappedItem(transform.position, out BaseGridView inventoryView)
                && InventoryView != inventoryView.InventoryPresenter)
            {
                InventoryView = inventoryView.InventoryPresenter;
            }

            if (InventoryView != null)
            {
                ColorizeItem(InventoryView);
            }
        }

        protected virtual void ColorizeItem(IInventoryPresenter inventoryView)
        {
            if (inventoryView != null && inventoryView.IsAvailablePlaceByCenter(tempData.TryGetItemSize(), transform.position))
            {
                Facade.ColorizeProvider.ColorizeToCoreect();
            }
            else
            {
                Facade.ColorizeProvider.ColorizeToIncoreect();
            }
        }

        protected virtual void ResetPosition()
        {
            placementItem.RotationIndex = rotationCount;
            if (InventoryView == null || !InventoryView.IsAvailablePlaceByCenter(tempData.TryGetItemSize(), transform.position))
            {
                transform.SetParent(startParent);
                transform.position = startPosition;
                placementItem.RotationIndex = startRotationCount;
                ApplyRotation(startRotationCount);
                if (InventoryView != null && !InventoryView.TryRestorePlacement(placementItem))
                {
                    InventoryView = null;
                }
            }
            else
            {
                ItemData = tempData;
                InventoryView?.TryPlaceItem(this);
            }
        }

        protected override void UpdateRotationView()
        {
            tempData = tempData.GetRotation(1);
            RotateBody();
            CheckAvaillablePlaced();
        }

        protected abstract void RotateBody();

        protected abstract void ConfigureBody();

        #endregion
    }
}