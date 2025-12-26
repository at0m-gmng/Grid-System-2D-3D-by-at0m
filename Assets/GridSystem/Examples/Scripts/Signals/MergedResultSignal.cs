namespace GridSystem.Signals
{
    using InventoryPresenters;
    using InventorySystem.Inventory.Data;

    public sealed class MergedResultSignal
    {
        public IInventoryPresenter InventoryView;
        public ItemData MergedResult;
    }
}