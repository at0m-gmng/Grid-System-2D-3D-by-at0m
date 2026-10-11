namespace GridSystem.Runtime.Interaction.Drop
{
    /// <summary>
    /// Orders of built-in drop handlers.
    /// </summary>
    public static class DropHandlerOrder
    {
        /// <summary>
        /// Merge with the target item.
        /// </summary>
        public const int MERGE = 100;

        /// <summary>
        /// Plain placement into free cells.
        /// </summary>
        public const int PLACE = 1000;
    }
}
