namespace GridSystem.Samples.Bootstrap
{
    using System;
    using Core.Inventories;
    using Runtime.Composition;
    using Runtime.Inventories;
    using Runtime.Persistence;
    using Runtime.Surfaces;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Creates sample inventories, restores saves, fills hand and world on the first launch.
    /// Toggles main, chest and world surfaces on input. Hand is always open.
    /// </summary>
    public sealed class SampleBootstrap : IGridInitializable, IDisposable
    {
        public SampleBootstrap(SampleSceneData data, IInventoryService inventories, ISurfaceService surfaces,
            IInventoryPersistence persistence, IGridItemFactory factory)
        {
            _data = data;
            _inventories = inventories;
            _surfaces = surfaces;
            _persistence = persistence;
            _factory = factory;
            _onToggleMain = _ => Toggle(_data.Main.Surface, _main);
            _onToggleChest = _ => Toggle(_data.Chest.Surface, _chest);
            _onToggleWorld = _ => Toggle(_data.World.Surface, _world);
        }

        private readonly SampleSceneData _data;
        private readonly IInventoryService _inventories;
        private readonly ISurfaceService _surfaces;
        private readonly IInventoryPersistence _persistence;
        private readonly IGridItemFactory _factory;
        private readonly Action<InputAction.CallbackContext> _onToggleMain;
        private readonly Action<InputAction.CallbackContext> _onToggleChest;
        private readonly Action<InputAction.CallbackContext> _onToggleWorld;
        private GridInventory _main;
        private GridInventory _chest;
        private GridInventory _hand;
        private GridInventory _world;

        /// <summary>
        /// Builds sample state: creates inventories, restores saves, opens hand surface, subscribes to toggles.
        /// </summary>
        public void Initialize()
        {
            _main = _inventories.Create(_data.Main.Definition, _data.Main.Key);
            _chest = _inventories.Create(_data.Chest.Definition, _data.Chest.Key);
            _hand = _inventories.Create(_data.Hand.Definition, _data.Hand.Key);
            _world = _inventories.Create(_data.World.Definition, _data.World.Key);

            if (!_persistence.TryLoad(_main))
            {
                Fill(_main, _data.MainStartItems);
            }

            _persistence.TryLoad(_chest);

            if (!_persistence.TryLoad(_hand))
            {
                Fill(_hand, _data.HandStartItems);
            }

            if (!_persistence.TryLoad(_world))
            {
                Fill(_world, _data.WorldStartItems);
            }

            _surfaces.Open(_data.Hand.Surface, _hand);

            _data.ToggleMain.performed += _onToggleMain;
            _data.ToggleChest.performed += _onToggleChest;
            _data.ToggleWorld.performed += _onToggleWorld;
            _data.ToggleMain.Enable();
            _data.ToggleChest.Enable();
            _data.ToggleWorld.Enable();
        }

        /// <summary>
        /// Saves all inventories and unsubscribes from toggles.
        /// </summary>
        public void Dispose()
        {
            _data.ToggleMain.performed -= _onToggleMain;
            _data.ToggleChest.performed -= _onToggleChest;
            _data.ToggleWorld.performed -= _onToggleWorld;
            _persistence.Save(_main);
            _persistence.Save(_chest);
            _persistence.Save(_hand);
            _persistence.Save(_world);
        }

        private void Toggle(GridSurface surface, GridInventory inventory)
        {
            if (_surfaces.IsOpen(surface))
            {
                _surfaces.Close(surface);
            }
            else
            {
                _surfaces.Open(surface, inventory);
            }
        }

        private void Fill(GridInventory inventory, SampleStartItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                SampleStartItem item = items[i];
                inventory.TryAdd(_factory.Create(item.Definition), item.Origin, item.Rotation);
            }
        }
    }
}
