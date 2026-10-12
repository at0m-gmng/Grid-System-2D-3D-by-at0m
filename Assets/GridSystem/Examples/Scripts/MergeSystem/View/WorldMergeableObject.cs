namespace GridSystem.MergeSystem.View
{
    using DragDropSystem.View;
    using IdentificationSystem;
    using OverlappedSystem;
    using OverlappedSystem.Data;
    using Zenject;

    public sealed class WorldMergeableObject : WorldDraggableObject
    {
        [Inject]
        private void Construct(IIdentificationSystem identificationSystem, WorldOverlapSystem overlapSystem, IMergeHandler mergeHandler)
        {
            this.identificationSystem = identificationSystem;
            this.overlapSystem = overlapSystem;
            this.mergeHandler = mergeHandler;
        }

        protected IMergeHandler mergeHandler;

        public override void EndDrag()
        {
            if (IsMergeableObject(out IPoolableDraggableObject overlappedItem))
            {
                InitializeMerge(overlappedItem);
            }
            else
            {
                ResetPosition();
            }

            Facade.ColorizeProvider.ColorizeToBase();
            tempData = default;
        }

        #region PRIVATE_REGION

        protected bool IsMergeableObject(out IPoolableDraggableObject overlappedItem)
        {
            overlappedItem = null;
            if (ItemData.IsMergable
                && overlapSystem.TryGetOverlappedItemInParent(
                    new OverlapEventData(transform.position, gameObject.GetInstanceID()),
                    ValidateOverlappedItem,
                    out WorldDraggableObject targetOveplappedItem))
            {
                overlappedItem = targetOveplappedItem;
                return true;
            }

            return false;
        }

        protected bool ValidateOverlappedItem(IPoolableDraggableObject targetObject) => targetObject.ItemData.IsMergable && targetObject != this;

        protected void InitializeMerge(IPoolableDraggableObject overlappedItem)
        {
            if (overlappedItem != null && mergeHandler.TryAddToMergeQueue(this, overlappedItem, overlappedItem.InventoryView))
            {
                InventoryView.TryReleasePlacement(placementItem);
                InventoryView.TryReleasePlacement(overlappedItem.PlacementItem);

                mergeHandler.Merge();
            }
            else
            {
                ResetPosition();
            }
        }

        #endregion
    }
}