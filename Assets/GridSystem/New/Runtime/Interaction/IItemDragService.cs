namespace GridSystem.Runtime.Interaction
{
    /// <summary>
    /// Drag state access for other systems.
    /// </summary>
    public interface IItemDragService
    {
        /// <summary>
        /// True while an item is dragged.
        /// </summary>
        public bool IsDragging { get; }

        /// <summary>
        /// Cancels the current drag without dropping.
        /// </summary>
        public void Cancel();
    }
}
