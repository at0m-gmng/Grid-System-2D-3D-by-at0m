namespace GridSystem.Core.Items
{
    /// <summary>
    /// Allocation free incremental identifier provider.
    /// </summary>
    public sealed class SequentialIdProvider
    {
        private int _last;

        /// <summary>
        /// Returns next unique identifier greater than zero.
        /// </summary>
        public int Next() => ++_last;
    }
}
