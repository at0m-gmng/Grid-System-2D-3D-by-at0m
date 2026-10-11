namespace GridSystem.Runtime.Surfaces
{
    using UnityEngine;

    /// <summary>
    /// 3D surface lying on the local XZ plane of this transform. The transform is the grid center.
    /// Rows go along -Z, columns along +X. Picking uses plane math, no physics.
    /// </summary>
    public sealed class WorldGridSurface : GridSurface
    {
        private const float QUARTER_TURN = 90f;
        private const float HALF = 0.5f;

        [SerializeField]
        private Camera _camera;

        /// <summary>
        /// World rotation for clockwise quarter turns.
        /// </summary>
        public override Quaternion GetRotation(int rotation) => transform.rotation * Quaternion.Euler(0f, QUARTER_TURN * rotation, 0f);

        protected override bool TryGetGridPoint(Vector2 screenPosition, out Vector2 gridPoint)
        {
            gridPoint = default;
            Ray ray = _camera.ScreenPointToRay(screenPosition);
            Plane plane = new Plane(transform.up, transform.position);
            bool result = plane.Raycast(ray, out float distance);
            if (result)
            {
                Vector3 local = transform.InverseTransformPoint(ray.GetPoint(distance));
                Vector2 halfSize = Size * HALF;
                gridPoint = new Vector2(local.x + halfSize.x, halfSize.y - local.z);
            }

            return result;
        }

        protected override Vector3 GridToWorld(Vector2 gridPoint)
        {
            Vector2 halfSize = Size * HALF;
            return transform.TransformPoint(new Vector3(gridPoint.x - halfSize.x, 0f, halfSize.y - gridPoint.y));
        }
    }
}
