namespace GridSystem.CreatedSystem
{
    using Data;
    using DragDropSystem.View;
    using Factories;
    using InventorySystem.Inventory.Data;
    using Zenject;

    public sealed class WorldSpawnSystem : AbstractSpawnSystem<WorldDraggableFactory, WorldDraggableObject, WorldItemsData>
    {
        public WorldSpawnSystem(SpawnSystemArguments spawnSystemArguments, SignalBus signalBus, WorldDraggableFactory itemViewFactory, WorldItemsData itemsData)
            : base(spawnSystemArguments, signalBus, itemViewFactory, itemsData)
        {
        }
    }
}