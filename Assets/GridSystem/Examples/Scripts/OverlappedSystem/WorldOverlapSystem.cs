namespace OverlappedSystem
{
    using System;
    using Data;
    using UnityEngine;

    public sealed class WorldOverlapSystem : AbstractOverlapSystem
    {
        public WorldOverlapSystem(WorldOverlapConfig config) => _config = config;

        private readonly WorldOverlapConfig _config;

        private RaycastHit[] _hitsBuffer = new RaycastHit[10];
        private Ray _ray;

        public override bool TryGetOverlappedItem<T>(Vector3 screenPosition, out T overlappedItem)
        {
            overlappedItem = default;

            _ray = _config.TargetCamera.ScreenPointToRay(screenPosition);
            int hitCount = Physics.RaycastNonAlloc(_ray, _hitsBuffer, _config.MaxDistance, _config.LayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                overlappedItem = _hitsBuffer[i].collider.GetComponent<T>();
                if (overlappedItem != null)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool TryGetOverlappedItemInParent<T>(Vector3 screenPosition, out T overlappedItem)
        {
            overlappedItem = default;

            _ray = _config.TargetCamera.ScreenPointToRay(screenPosition);
            int hitCount = Physics.RaycastNonAlloc(_ray, _hitsBuffer, _config.MaxDistance, _config.LayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                overlappedItem = _hitsBuffer[i].collider.GetComponentInParent<T>();
                if (overlappedItem != null)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool TryGetOverlappedItem<T>(Vector3 screenPosition, Func<GameObject, bool> validationFilter, out T overlappedItem)
        {
            overlappedItem = default;

            _ray = _config.TargetCamera.ScreenPointToRay(screenPosition);
            int hitCount = Physics.RaycastNonAlloc(_ray, _hitsBuffer, _config.MaxDistance, _config.LayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = _hitsBuffer[i];
                overlappedItem = hit.collider.GetComponent<T>();
                if (overlappedItem != null && validationFilter(hit.collider.gameObject))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool TryGetOverlappedItemInParent<T>(Vector3 screenPosition, Func<GameObject, bool> validationFilter, out T overlappedItem)
        {
            overlappedItem = default;

            _ray = _config.TargetCamera.ScreenPointToRay(screenPosition);
            int hitCount = Physics.RaycastNonAlloc(_ray, _hitsBuffer, _config.MaxDistance, _config.LayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = _hitsBuffer[i];
                overlappedItem = hit.collider.GetComponentInParent<T>();
                if (overlappedItem != null && validationFilter(hit.collider.gameObject))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool TryGetOverlappedItem<T>(OverlapEventData overlapEventData, Func<T, bool> validationFilter, out T overlappedItem)
        {
            overlappedItem = default;

            Vector3 screenPosition = overlapEventData.Position.z != 0
                ? _config.TargetCamera.WorldToScreenPoint(overlapEventData.Position)
                : overlapEventData.Position;

            _ray = _config.TargetCamera.ScreenPointToRay(screenPosition);
            int hitCount = Physics.RaycastNonAlloc(_ray, _hitsBuffer, _config.MaxDistance, _config.LayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                if (_hitsBuffer[i].collider.gameObject.GetInstanceID() != overlapEventData.GameObjectID)
                {
                    overlappedItem = _hitsBuffer[i].collider.GetComponent<T>();
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

            Vector3 screenPosition = overlapEventData.Position.z != 0
                ? _config.TargetCamera.WorldToScreenPoint(overlapEventData.Position)
                : overlapEventData.Position;

            _ray = _config.TargetCamera.ScreenPointToRay(screenPosition);
            int hitCount = Physics.RaycastNonAlloc(_ray, _hitsBuffer, _config.MaxDistance, _config.LayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                if (_hitsBuffer[i].collider.gameObject.GetInstanceID() != overlapEventData.GameObjectID)
                {
                    overlappedItem = _hitsBuffer[i].collider.GetComponentInParent<T>();
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