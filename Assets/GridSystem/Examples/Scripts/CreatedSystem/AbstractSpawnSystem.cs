namespace GridSystem.CreatedSystem
{
    using Data;
    using DragDropSystem.View;
    using Factories;
    using InventorySystem.Inventory.Data;
    using Signals;
    using Zenject;

    public abstract class AbstractSpawnSystem<TFactory, TItem, TConfig> : IInitializable
        where TFactory : PlaceholderFactory<DraggableData, TItem>
        where TItem : IPoolableDraggableObject
        where TConfig : ItemsData
    {
        public AbstractSpawnSystem(SpawnSystemArguments spawnSystemArguments, SignalBus signalBus, TFactory itemViewFactory,
            TConfig itemsData)
        {
            _spawnSystemArguments = spawnSystemArguments;
            _signalBus = signalBus;
            _itemViewFactory = itemViewFactory;
            _itemsData = itemsData;
        }

        private readonly SpawnSystemArguments _spawnSystemArguments;
        private readonly SignalBus _signalBus;
        private readonly TFactory _itemViewFactory;
        private readonly TConfig _itemsData;

        public void Initialize()
        {
            _signalBus.Subscribe<MergedResultSignal>(InitializeCreatingMergeItem);
            _signalBus.Subscribe<DataSelectedForSpawnSignal>(InitializeCreatingSelectableItems);
            _signalBus.Subscribe<InventoryViewSpawnSignal>(CreateItemsForPresenter);
        }

        private void InitializeCreatingMergeItem(MergedResultSignal signal)
        {
            ItemData itemData = _itemsData.Item.Find(x => x.Item.Type == signal.MergedResult.Item.Type);

            if (itemData != null)
            {
                DraggableData uiDraggableData = new DraggableData(signal.MergedResult, _spawnSystemArguments.TemporatyTransform, signal.InventoryView);
                _signalBus.Fire(new CreatedItemSignal
                {
                    UIDraggableObject = _itemViewFactory.Create(uiDraggableData)
                });
            }
        }

        private void InitializeCreatingSelectableItems(DataSelectedForSpawnSignal signal)
        {
            foreach (ItemData item in signal.SelectedForSpawn)
            {
                ItemData itemData = _itemsData.Item.Find(x => x.Item.Type == item.Item.Type);

                if (itemData != null)
                {
                    DraggableData uiDraggableData = new DraggableData(item, _spawnSystemArguments.TemporatyTransform);
                    _signalBus.Fire(new SelectedItemSignal
                    {
                        UIDraggableObject = _itemViewFactory.Create(uiDraggableData)
                    });
                }
            }
        }

        private void CreateItemsForPresenter(InventoryViewSpawnSignal data) => CreateItemsForPresenter(data.SpawnData);

        private void CreateItemsForPresenter(SpawnData data)
        {
            foreach (PlacementItem placementItem in data.PlacementItems)
            {
                ItemData itemData = _itemsData.Item.Find(x => x.Item.Type == placementItem.ID);
                if (itemData != null)
                {
                    DraggableData uiDraggableData = new DraggableData(itemData, _spawnSystemArguments.TemporatyTransform, data.InventoryPresenter, placementItem);
                    data.InventoryPresenter.TryRestorePlacement(_itemViewFactory.Create(uiDraggableData));
                }
            }
        }
    }
}