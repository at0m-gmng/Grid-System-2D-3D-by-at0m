namespace GridSystem.Core.Items
{
    using Shapes;

    /// <summary>
    /// Static item data shared by all item instances.
    /// </summary>
    public interface IGridItemDefinition
    {
        /// <summary>
        /// Persistent unique identifier.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Occupied cells.
        /// </summary>
        public GridShape Shape { get; }

        /// <summary>
        /// Allows rotation by the user and auto placement.
        /// </summary>
        public bool IsRotatable { get; }
    }
}
