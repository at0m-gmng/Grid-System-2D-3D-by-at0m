namespace OverlappedSystem
{
    using System;
    using System.Collections.Generic;
    using Data;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public sealed class UIOverlapSystem : AbstractOverlapSystem
    {
        private List<RaycastResult> _rayResult = new List<RaycastResult>();

        public override bool TryGetOverlappedItem<T>(Vector3 position, out T overlappedItem)
        {
            overlappedItem = default;
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = position
            };
            _rayResult.Clear();
            EventSystem.current.RaycastAll(pointerData, _rayResult);

            foreach (RaycastResult result in _rayResult)
            {
                overlappedItem = result.gameObject.GetComponent<T>();
                if (overlappedItem != null)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool TryGetOverlappedItemInParent<T>(Vector3 position, out T overlappedItem)
        {
            overlappedItem = default;
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = position
            };
            _rayResult.Clear();
            EventSystem.current.RaycastAll(pointerData, _rayResult);

            foreach (RaycastResult result in _rayResult)
            {
                overlappedItem = result.gameObject.GetComponentInParent<T>();
                if (overlappedItem != null)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool TryGetOverlappedItem<T>(Vector3 position, Func<GameObject, bool> validationFilter, out T overlappedItem)
        {
            overlappedItem = default;
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = position
            };
            _rayResult.Clear();
            EventSystem.current.RaycastAll(pointerData, _rayResult);

            foreach (RaycastResult result in _rayResult)
            {
                overlappedItem = result.gameObject.GetComponent<T>();
                if (overlappedItem != null && validationFilter(result.gameObject))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool TryGetOverlappedItemInParent<T>(Vector3 position, Func<GameObject, bool> validationFilter, out T overlappedItem)
        {
            overlappedItem = default;
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = position
            };
            _rayResult.Clear();
            EventSystem.current.RaycastAll(pointerData, _rayResult);

            foreach (RaycastResult result in _rayResult)
            {
                overlappedItem = result.gameObject.GetComponentInParent<T>();
                if (overlappedItem != null && validationFilter(result.gameObject))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool TryGetOverlappedItem<T>(OverlapEventData overlapEventData, Func<T, bool> validationFilter, out T overlappedItem)
        {
            overlappedItem = default;
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = overlapEventData.Position
            };
            _rayResult.Clear();
            EventSystem.current.RaycastAll(pointerData, _rayResult);

            foreach (RaycastResult result in _rayResult)
            {
                if (result.gameObject.GetInstanceID() != overlapEventData.GameObjectID)
                {
                    overlappedItem = result.gameObject.GetComponent<T>();
                    if (overlappedItem != null && validationFilter(overlappedItem))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool TryGetOverlappedItemInParent<T>(OverlapEventData overlapEventData, Func<T, bool> validationFilter, out T overlappedItem)
        {
            overlappedItem = default;
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = overlapEventData.Position
            };
            _rayResult.Clear();
            EventSystem.current.RaycastAll(pointerData, _rayResult);

            foreach (RaycastResult result in _rayResult)
            {
                if (result.gameObject.GetInstanceID() != overlapEventData.GameObjectID)
                {
                    overlappedItem = result.gameObject.GetComponentInParent<T>();
                    if (overlappedItem != null && validationFilter(overlappedItem))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}