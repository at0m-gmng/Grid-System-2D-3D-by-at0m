namespace GridSystem.MergeSystem
{
    using System;
    using System.Collections.Generic;
    using DragDropSystem.View;
    using InventoryPresenters;
    using InventorySystem.Inventory.Data;
    using Signals;
    using Zenject;

    public sealed class MergeHandler : IMergeHandler, IDisposable
    {
        public MergeHandler(MergeConfig mergeConfig, SignalBus signalBus)
        {
            _mergeConfig = mergeConfig;
            _signalBus = signalBus;
        }

        private readonly MergeConfig _mergeConfig;
        private readonly SignalBus _signalBus;

        private Dictionary<(IPoolableDraggableObject, IPoolableDraggableObject), (IInventoryPresenter, ItemData)> _itemViews = new();

        public bool TryAddToMergeQueue(IPoolableDraggableObject firstItem, IPoolableDraggableObject secondItem, IInventoryPresenter inventoryView)
        {
            if (firstItem.ItemData.Type == secondItem.ItemData.Type && firstItem.ItemData.Level == secondItem.ItemData.Level)
            {
                foreach (MergeRule rule in _mergeConfig.Rules)
                {
                    if (rule.Type == firstItem.ItemData.Type && rule.Level == firstItem.ItemData.Level)
                    {
                        _itemViews.Add(new(firstItem, secondItem), (inventoryView, rule.ResultItem));
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        public void Merge()
        {
            foreach (KeyValuePair<(IPoolableDraggableObject, IPoolableDraggableObject), (IInventoryPresenter, ItemData)> mergeData in _itemViews)
            {
                _signalBus.Fire(new ItemMergedSignal
                {
                    FirstItem = mergeData.Key.Item1,
                    SecondItem = mergeData.Key.Item2
                });
                _signalBus.Fire(new MergedResultSignal
                {
                    InventoryView = mergeData.Value.Item1,
                    MergedResult = mergeData.Value.Item2
                });
            }

            _itemViews.Clear();
        }

        public void Dispose() => _itemViews.Clear();
    }

    public interface IMergeHandler
    {
        public bool TryAddToMergeQueue(IPoolableDraggableObject firstItem, IPoolableDraggableObject secondItem, IInventoryPresenter inventoryView);

        public void Merge();
    }
}