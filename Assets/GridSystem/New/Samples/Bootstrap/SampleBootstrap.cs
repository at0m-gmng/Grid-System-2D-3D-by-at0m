namespace GridSystem.Samples.Bootstrap
{
    using System;
    using Core.Inventories;
    using Runtime.Composition;
    using Runtime.Inventories;
    using Runtime.Persistence;
    using Runtime.Surfaces;
    using UnityEngine.InputSystem;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Creates sample inventories, restores saves, fills empty ones and toggles the main window.
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
            _onToggle = OnToggle;
        }

        private readonly SampleSceneData _data;
        private readonly IInventoryService _inventories;
        private readonly ISurfaceService _surfaces;
        private readonly IInventoryPersistence _persistence;
        private readonly IGridItemFactory _factory;
        private readonly Action<InputAction.CallbackContext> _onToggle;
        private GridInventory _main;
        private GridInventory _hand;
        private GridInventory _world;

        /// <summary>
        /// Builds sample state.
        /// </summary>
        public void Initialize()
        {
            _main = Create(_data.Main);
            _hand = Create(_data.Hand);
            _world = Create(_data.World);
            FillWorld();
            FillHand();
            _surfaces.Open(_data.Hand.Surface, _hand);
            _surfaces.Open(_data.World.Surface, _world);
            SetMainWindowOpened(_data.IsMainWindowOpened);
            _data.ToggleMainWindow.performed += _onToggle;
            _data.ToggleMainWindow.Enable();
        }

        /// <summary>
        /// Saves inventories and unsubscribes.
        /// </summary>
        public void Dispose()
        {
            _data.ToggleMainWindow.performed -= _onToggle;
            _persistence.Save(_main);
            _persistence.Save(_hand);
            _persistence.Save(_world);
        }

        private GridInventory Create(SampleInventory sample)
        {
            GridInventory inventory = _inventories.Create(sample.Definition, sample.Key);
            _persistence.TryLoad(inventory);
            return inventory;
        }

        private void FillWorld()
        {
            if (_world.Count == 0)
            {
                for (int i = 0; i < _data.WorldStartItems.Length; i++)
                {
                    SampleStartItem item = _data.WorldStartItems[i];
                    _world.TryAdd(_factory.Create(item.Definition), item.Origin, item.Rotation);
                }
            }
        }

        private void FillHand()
        {
            if (_hand.Count == 0)
            {
                for (int i = 0; i < _data.HandSpawnCount; i++)
                {
                    _hand.TryAdd(_factory.Create(_data.HandSpawnPool[Random.Range(0, _data.HandSpawnPool.Length)]));
                }
            }
        }

        private void OnToggle(InputAction.CallbackContext context) => SetMainWindowOpened(!_surfaces.IsOpen(_data.Main.Surface));

        private void SetMainWindowOpened(bool isOpened)
        {
            _data.MainWindow.SetActive(isOpened);
            if (isOpened)
            {
                _surfaces.Open(_data.Main.Surface, _main);
            }
            else
            {
                _surfaces.Close(_data.Main.Surface);
                _persistence.Save(_main);
            }
        }
    }
}
