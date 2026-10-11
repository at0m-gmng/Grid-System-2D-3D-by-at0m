namespace GridSystem.Samples.Bootstrap
{
    using System;
    using Core.Inventories;
    using Runtime.Composition;
    using Runtime.Inventories;
    using Runtime.Persistence;
    using Runtime.Surfaces;
    using UnityEngine;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Creates sample inventories, restores saves, fills inventories on the first launch and binds window toggles.
    /// Window visibility and saving on close react to surface messages in separate classes.
    /// </summary>
    public sealed class SampleBootstrap : IGridInitializable, IDisposable
    {
        public SampleBootstrap(SampleSceneData data, IInventoryService inventories, ISurfaceService surfaces, IInventoryPersistence persistence,
            IGridItemFactory factory)
        {
            _data = data;
            _inventories = inventories;
            _surfaces = surfaces;
            _persistence = persistence;
            _factory = factory;
            _windows = new SampleWindowController[data.Windows.Length];
        }

        private readonly SampleSceneData _data;
        private readonly IInventoryService _inventories;
        private readonly ISurfaceService _surfaces;
        private readonly IInventoryPersistence _persistence;
        private readonly IGridItemFactory _factory;
        private readonly SampleWindowController[] _windows;
        private GridInventory _hand;

        /// <summary>
        /// Builds sample state.
        /// </summary>
        public void Initialize()
        {
            _hand = _inventories.Create(_data.Hand.Definition, _data.Hand.Key);
            if (!_persistence.TryLoad(_hand))
            {
                FillHand();
            }

            _surfaces.Open(_data.Hand.Surface, _hand);
            for (int i = 0; i < _windows.Length; i++)
            {
                SampleWindow window = _data.Windows[i];
                GridInventory inventory = _inventories.Create(window.Inventory.Definition, window.Inventory.Key);
                if (!_persistence.TryLoad(inventory))
                {
                    Fill(inventory, window.StartItems);
                }

                _windows[i] = new SampleWindowController(window, inventory, _surfaces);
                _windows[i].Bind();
            }
        }

        /// <summary>
        /// Saves inventories and unbinds toggles.
        /// </summary>
        public void Dispose()
        {
            _persistence.Save(_hand);
            for (int i = 0; i < _windows.Length; i++)
            {
                _windows[i].Unbind();
                _persistence.Save(_windows[i].Inventory);
            }
        }

        private void Fill(GridInventory inventory, SampleStartItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                inventory.TryAdd(_factory.Create(items[i].Definition), items[i].Origin, items[i].Rotation);
            }
        }

        private void FillHand()
        {
            int poolLength = _data.HandSpawnPool.Length;
            int count = Mathf.Min(_data.HandSpawnCount, poolLength);
            int start = Random.Range(0, poolLength);
            for (int i = 0; i < count; i++)
            {
                _hand.TryAdd(_factory.Create(_data.HandSpawnPool[(start + i) % poolLength]));
            }
        }
    }
}
