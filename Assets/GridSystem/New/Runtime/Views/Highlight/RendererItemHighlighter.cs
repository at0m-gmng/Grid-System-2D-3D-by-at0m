namespace GridSystem.Runtime.Views.Highlight
{
    using UnityEngine;

    /// <summary>
    /// Tints renderers through a property block, materials are not instantiated.
    /// Normal state restores material colors.
    /// </summary>
    public sealed class RendererItemHighlighter : ItemHighlighter
    {
        private const string DEFAULT_COLOR_PROPERTY = "_BaseColor";

        [SerializeField]
        private Renderer[] _renderers;

        [Tooltip("Shader color property: _BaseColor for URP/HDRP, _Color for Built-in.")]
        [SerializeField]
        private string _colorProperty = DEFAULT_COLOR_PROPERTY;

        private MaterialPropertyBlock _block;
        private int _propertyId;

        /// <summary>
        /// Applies state color.
        /// </summary>
        public override void SetState(PlacementState state)
        {
            if (_block == null)
            {
                _block = new MaterialPropertyBlock();
                _propertyId = Shader.PropertyToID(_colorProperty);
            }

            _block.Clear();
            if (state != PlacementState.Normal)
            {
                _block.SetColor(_propertyId, palette.Get(state));
            }

            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].SetPropertyBlock(_block);
            }
        }
    }
}
