namespace GridSystem.Runtime.Views.Highlight
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Colors of placement states.
    /// </summary>
    [Serializable]
    public struct PlacementPalette
    {
        public PlacementPalette(Color normal, Color valid, Color invalid)
        {
            _normal = normal;
            _valid = valid;
            _invalid = invalid;
        }

        [SerializeField]
        private Color _normal;

        [SerializeField]
        private Color _valid;

        [SerializeField]
        private Color _invalid;

        /// <summary>
        /// White, green and red palette.
        /// </summary>
        public static PlacementPalette Default => new PlacementPalette(Color.white, Color.green, Color.red);

        /// <summary>
        /// Returns color of the state.
        /// </summary>
        public Color Get(PlacementState state) => state switch
        {
            PlacementState.Valid => _valid,
            PlacementState.Invalid => _invalid,
            _ => _normal
        };
    }
}
