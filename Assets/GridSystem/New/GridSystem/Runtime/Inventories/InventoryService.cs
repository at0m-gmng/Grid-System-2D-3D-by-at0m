namespace GridSystem.Runtime.Inventories
{
    using System;
    using System.Collections.Generic;
    using Core.Inventories;
    using Core.Items;
    using Definitions;
    using MessagePipe;
    using Messaging;
    using R3;

    /// <summary>
    /// Creates and tracks inventories. Relays their changes to the message bus.
    /// </summary>
    public sealed class InventoryService : IInventoryService, IDisposable
    {
        public InventoryService(IPublisher<ItemAddedMessage> addedPublisher, IPublisher<ItemRemovedMessage> removedPublisher,
            IPublisher<ItemMovedMessage> movedPublisher)
        {
            _addedPublisher = addedPublisher;
            _removedPublisher = removedPublisher;
            _movedPublisher = movedPublisher;
        }

        private readonly IPublisher<ItemAddedMessage> _addedPublisher;
        private readonly IPublisher<ItemRemovedMessage> _removedPublisher;
        private readonly IPublisher<ItemMovedMessage> _movedPublisher;
        private readonly SequentialIdProvider _ids = new();
        private readonly Dictionary<int, InventoryEntry> _inventories = new();

        /// <summary>
        /// Creates an empty inventory.
        /// </summary>
        public GridInventory Create(GridDefinition definition, string key)
        {
            GridInventory inventory = new GridInventory(_ids.Next(), key, definition.Width, definition.Height);
            IDisposable subscriptions = Disposable.Combine(
                inventory.Added.Subscribe((Service: this, Inventory: inventory), static (item, state) => state.Service.OnAdded(state.Inventory, item)),
                inventory.Removed.Subscribe((Service: this, Inventory: inventory), static (item, state) => state.Service.OnRemoved(state.Inventory, item)),
                inventory.Moved.Subscribe((Service: this, Inventory: inventory), static (movement, state) => state.Service.OnMoved(state.Inventory, movement)));
            _inventories.Add(inventory.Id, new InventoryEntry(inventory, subscriptions));
            return inventory;
        }

        /// <summary>
        /// Clears and forgets the inventory.
        /// </summary>
        public void Destroy(GridInventory inventory)
        {
            if (_inventories.Remove(inventory.Id, out InventoryEntry entry))
            {
                inventory.Clear();
                entry.Dispose();
            }
        }

        /// <summary>
        /// Disposes every inventory.
        /// </summary>
        public void Dispose()
        {
            foreach (InventoryEntry entry in _inventories.Values)
            {
                entry.Dispose();
            }

            _inventories.Clear();
        }

        private void OnAdded(GridInventory inventory, GridItem item) => _addedPublisher.Publish(new ItemAddedMessage(inventory, item));

        private void OnRemoved(GridInventory inventory, GridItem item) => _removedPublisher.Publish(new ItemRemovedMessage(inventory, item));

        private void OnMoved(GridInventory inventory, ItemMovement movement) => _movedPublisher.Publish(new ItemMovedMessage(inventory, movement));

        private readonly struct InventoryEntry
        {
            public InventoryEntry(GridInventory inventory, IDisposable subscriptions)
            {
                Inventory = inventory;
                Subscriptions = subscriptions;
            }

            public readonly GridInventory Inventory;
            public readonly IDisposable Subscriptions;

            public void Dispose()
            {
                Subscriptions.Dispose();
                Inventory.Dispose();
            }
        }
    }
}
