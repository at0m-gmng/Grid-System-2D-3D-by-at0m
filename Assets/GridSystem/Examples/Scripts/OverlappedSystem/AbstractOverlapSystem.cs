namespace OverlappedSystem
{
    using System;
    using Data;
    using UnityEngine;

    public abstract class AbstractOverlapSystem
    {
        public abstract bool TryGetOverlappedItem<T>(Vector3 position, out T overlappedItem);
        public abstract bool TryGetOverlappedItemInParent<T>(Vector3 position, out T overlappedItem);
        public abstract bool TryGetOverlappedItem<T>(Vector3 position, Func<GameObject, bool> validationFilter, out T overlappedItem);
        public abstract bool TryGetOverlappedItemInParent<T>(Vector3 position, Func<GameObject, bool> validationFilter, out T overlappedItem);
        public abstract bool TryGetOverlappedItem<T>(OverlapEventData overlapEventData, Func<T, bool> validationFilter, out T overlappedItem);
        public abstract bool TryGetOverlappedItemInParent<T>(OverlapEventData overlapEventData, Func<T, bool> validationFilter, out T overlappedItem);
    }
}