namespace GridSystem.Runtime.Merge
{
    using Core.Items;

    /// <summary>
    /// Merge recipes.
    /// </summary>
    public interface IMergeRules
    {
        /// <summary>
        /// Finds merge result for the pair in any order.
        /// </summary>
        public bool TryGetResult(IGridItemDefinition first, IGridItemDefinition second, out IGridItemDefinition result);
    }
}
