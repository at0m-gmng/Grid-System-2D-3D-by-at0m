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
        public SampleBootstrap(SampleSceneConfig config, SampleContent content, IInventoryService inventories,
            ISurfaceService surfaces, IInventoryPersistence persistence, IGridItemFactory factory)
        {
            _config = config;
            _content = content;
            _inventories = inventories;
            _surfaces = surfaces;
            _persistence = persistence;
            _factory = factory;
            _onToggleMain = _ => Toggle(_content.Ui.Main, _main);
            _onToggleChest = _ => Toggle(_content.Ui.Chest, _chest);
            _onToggleWorld = _ => Toggle(_content.World, _world);
        }

        private readonly SampleSceneConfig _config;
        private readonly SampleContent _content;
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
            _main = _inventories.Create(_config.Main.Definition, _config.Main.Key);
            _chest = _inventories.Create(_config.Chest.Definition, _config.Chest.Key);
            _hand = _inventories.Create(_config.Hand.Definition, _config.Hand.Key);
            _world = _inventories.Create(_config.World.Definition, _config.World.Key);

            if (!_persistence.TryLoad(_main))
            {
                Fill(_main, _config.MainStartItems);
            }

            _persistence.TryLoad(_chest);

            if (!_persistence.TryLoad(_hand))
            {
                Fill(_hand, _config.HandStartItems);
            }

            if (!_persistence.TryLoad(_world))
            {
                Fill(_world, _config.WorldStartItems);
            }

            _surfaces.Open(_content.Ui.Hand, _hand);

            _config.ToggleMain.performed += _onToggleMain;
            _config.ToggleChest.performed += _onToggleChest;
            _config.ToggleWorld.performed += _onToggleWorld;
            _config.ToggleMain.Enable();
            _config.ToggleChest.Enable();
            _config.ToggleWorld.Enable();
        }

        /// <summary>
        /// Saves all inventories and unsubscribes from toggles.
        /// </summary>
        public void Dispose()
        {
            _config.ToggleMain.performed -= _onToggleMain;
            _config.ToggleChest.performed -= _onToggleChest;
            _config.ToggleWorld.performed -= _onToggleWorld;
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
