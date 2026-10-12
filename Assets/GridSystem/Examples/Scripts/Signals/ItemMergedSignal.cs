namespace GridSystem.Signals
{
    using DragDropSystem.View;

    public sealed class ItemMergedSignal
    {
        public IPoolableDraggableObject FirstItem;
        public IPoolableDraggableObject SecondItem;
    }
}