namespace GridSystem.CreatedSystem.Data
{
    using System.Collections.Generic;
    using InventoryPresenters;
    using InventorySystem.Inventory.Data;

    public struct SpawnData
    {
        public IInventoryPresenter InventoryPresenter;
        public List<PlacementItem> PlacementItems;
    }
}