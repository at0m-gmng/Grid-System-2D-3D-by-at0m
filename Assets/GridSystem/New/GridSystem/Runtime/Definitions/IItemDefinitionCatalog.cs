namespace GridSystem.Runtime.Definitions
{
    using Core.Items;

    /// <summary>
    /// Resolves item definitions by persistent id.
    /// </summary>
    public interface IItemDefinitionCatalog
    {
        /// <summary>
        /// Finds definition by persistent id.
        /// </summary>
        public bool TryGet(string id, out IGridItemDefinition definition);
    }
}
