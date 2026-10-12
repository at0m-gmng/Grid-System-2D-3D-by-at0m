namespace OverlappedSystem.Data
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Configs/WorldOverlapConfig", fileName = "WorldOverlapConfig")]
    public sealed class WorldOverlapConfig : ScriptableObject
    {
        [field: SerializeField] public LayerMask LayerMask { get; private set; } = ~0;
        [field: SerializeField] public float MaxDistance { get; private set; } = 100f;

        public Camera TargetCamera
        {
            get
            {
                if (_targetCamera == null)
                {
                    _targetCamera = Camera.main;
                }

                return _targetCamera;
            }
        }

        private Camera _targetCamera;
    }
}