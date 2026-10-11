namespace GridSystem.Runtime.Inventories
{
    using Core.Items;

    /// <summary>
    /// Creates item instances with unique ids.
    /// </summary>
    public interface IGridItemFactory
    {
        /// <summary>
        /// Creates a new unplaced item.
        /// </summary>
        public GridItem Create(IGridItemDefinition definition);
    }
}
