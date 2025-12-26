namespace GridSystem.Signals
{
    using System.Collections.Generic;
    using InventorySystem.Inventory.Data;

    public sealed class DataSelectedForSpawnSignal
    {
        public List<ItemData> SelectedForSpawn;
    }
}