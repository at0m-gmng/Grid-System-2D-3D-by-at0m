namespace GridSystem.Signals
{
    using InventorySystem.Inventory.Core;

    public sealed class OpenInventorySignal
    {
        public bool IsOpen;
        public InventorySystemData InventorySystemData;
    }
}