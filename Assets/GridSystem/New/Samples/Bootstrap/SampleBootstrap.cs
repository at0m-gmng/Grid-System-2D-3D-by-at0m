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
            _main = Restore(_config.Main);
            _chest = Restore(_config.Chest);
            _hand = Restore(_config.Hand);
            _world = Restore(_config.World);
            _surfaces.Open(_content.Ui.Hand, _hand);

            _config.ToggleMain.performed += _onToggleMain;
            _config.ToggleChest.performed += _onToggleChest;
            _config.ToggleWorld.performed += _onToggleWorld;
            _config.ToggleMain.Enable();
            _config.ToggleChest.Enable();
            _config.ToggleWorld.Enable();
        }

        /// <summary>
        /// Saves persistent inventories, unsubscribes from toggles and disables the actions it enabled.
        /// The actions live in an asset, so leaving them enabled leaks into the next session.
        /// </summary>
        public void Dispose()
        {
            _config.ToggleMain.performed -= _onToggleMain;
            _config.ToggleChest.performed -= _onToggleChest;
            _config.ToggleWorld.performed -= _onToggleWorld;
            _config.ToggleMain.Disable();
            _config.ToggleChest.Disable();
            _config.ToggleWorld.Disable();
            Save(_config.Main, _main);
            Save(_config.Chest, _chest);
            Save(_config.Hand, _hand);
            Save(_config.World, _world);
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

        private GridInventory Restore(SampleInventory source)
        {
            GridInventory inventory = _inventories.Create(source.Definition, source.Key);
            if (!source.IsPersistent || !_persistence.TryLoad(inventory))
            {
                Fill(inventory, source.StartItems);
            }

            return inventory;
        }

        private void Save(SampleInventory source, GridInventory inventory)
        {
            if (source.IsPersistent)
            {
                _persistence.Save(inventory);
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
