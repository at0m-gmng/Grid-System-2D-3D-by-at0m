namespace GridSystem.Samples.Bootstrap
{
    using System;
    using Core.Inventories;
    using Runtime.Composition;
    using Runtime.Inventories;
    using Runtime.Persistence;
    using Runtime.Surfaces;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Creates sample inventories, restores saves, fills hand and world on the first launch and toggles the main surface.
    /// Window visibility and saving react to surface messages in separate classes.
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
            _main = _inventories.Create(_data.Main.Definition, _data.Main.Key);
            _hand = _inventories.Create(_data.Hand.Definition, _data.Hand.Key);
            _world = _inventories.Create(_data.World.Definition, _data.World.Key);
            _persistence.TryLoad(_main);
            if (!_persistence.TryLoad(_hand))
            {
                FillHand();
            }

            if (!_persistence.TryLoad(_world))
            {
                FillWorld();
            }

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

        private void FillWorld()
        {
            for (int i = 0; i < _data.WorldStartItems.Length; i++)
            {
                SampleStartItem item = _data.WorldStartItems[i];
                _world.TryAdd(_factory.Create(item.Definition), item.Origin, item.Rotation);
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

        private void OnToggle(InputAction.CallbackContext context) => SetMainWindowOpened(!_surfaces.IsOpen(_data.Main.Surface));

        private void SetMainWindowOpened(bool isOpened)
        {
            if (isOpened)
            {
                _surfaces.Open(_data.Main.Surface, _main);
            }
            else
            {
                _surfaces.Close(_data.Main.Surface);
            }
        }
    }
}
