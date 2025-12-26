namespace GridSystem.CreatedSystem
{
    using Data;
    using DragDropSystem.View;
    using Factories;
    using InventorySystem.Inventory.Data;
    using Zenject;

    public sealed class UISpawnSystem : AbstractSpawnSystem<UIDraggableFactory, UIDraggableObject, UIItemsData>
    {
        public UISpawnSystem(SpawnSystemArguments spawnSystemArguments, SignalBus signalBus, UIDraggableFactory itemViewFactory, UIItemsData itemsData)
            : base(spawnSystemArguments, signalBus, itemViewFactory, itemsData)
        {
        }
    }
}