namespace GridSystem.Samples.Definitions
{
    /// <summary>
    /// Data shown by the sample tooltip.
    /// </summary>
    public interface IItemDescription
    {
        /// <summary>
        /// Display name.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Damage value.
        /// </summary>
        public float Damage { get; }

        /// <summary>
        /// Attack speed value.
        /// </summary>
        public float AttackSpeed { get; }

        /// <summary>
        /// Range value.
        /// </summary>
        public float Range { get; }
    }
}
