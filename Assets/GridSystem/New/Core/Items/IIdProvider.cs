namespace GridSystem.Core.Items
{
    /// <summary>
    /// Issues unique runtime identifiers.
    /// </summary>
    public interface IIdProvider
    {
        /// <summary>
        /// Returns next unique identifier greater than zero.
        /// </summary>
        public int Next();
    }
}
