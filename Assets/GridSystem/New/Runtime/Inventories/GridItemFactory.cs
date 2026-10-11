namespace GridSystem.Runtime.Inventories
{
    using Core.Items;

    /// <summary>
    /// Creates item instances with sequential ids.
    /// </summary>
    public sealed class GridItemFactory : IGridItemFactory
    {
        private readonly SequentialIdProvider _ids = new();

        /// <summary>
        /// Creates a new unplaced item.
        /// </summary>
        public GridItem Create(IGridItemDefinition definition) => new GridItem(_ids.Next(), definition);
    }
}
