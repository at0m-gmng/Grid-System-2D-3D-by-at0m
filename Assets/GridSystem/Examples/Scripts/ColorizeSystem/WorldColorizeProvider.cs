namespace GridSystem.ColorizeSystem
{
    using UnityEngine;

    public sealed class WorldColorizeProvider : AbstractColorizeProvider
    {
        [SerializeField]
        private MeshRenderer _meshRenderer;

        private Material _material;

        private void Start()
        {
            _material = new Material(_meshRenderer.sharedMaterial);
            _meshRenderer.material = _material;
        }

        public override void ColorizeToCoreect()
            => _meshRenderer.sharedMaterial.color = ColorizeWithAlpha(Color.green, 0.8f);

        public override void ColorizeToIncoreect()
            => _meshRenderer.sharedMaterial.color = ColorizeWithAlpha(Color.red, 0.8f);

        public override void ColorizeToBase()
            => _meshRenderer.sharedMaterial.color = ColorizeWithAlpha(Color.white, 0f);
    }
}