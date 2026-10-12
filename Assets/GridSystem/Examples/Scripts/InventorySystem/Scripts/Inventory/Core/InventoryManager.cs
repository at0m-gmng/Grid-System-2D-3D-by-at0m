namespace GridSystem.InventorySystem.Inventory.Core
{
    using System;
    using System.Collections.Generic;
    using Configs;
    using Data;
    using Enums;
    using IdentificationSystem;
    using SaveSystem;
    using Signals;
    using Zenject;

    public sealed class InventoryManager : IInventoryManager
    {
        public InventoryManager(InventoriesConfig inventoriesConfig, IIdentificationSystem identificationSystem, ISaveLoadSystem saveLoadService,
            SignalBus signalBus)
        {
            _inventoriesConfig = inventoriesConfig;
            _identificationSystem = identificationSystem;
            _saveLoadService = saveLoadService;
            _signalBus = signalBus;
        }

        private readonly InventoriesConfig _inventoriesConfig;
        private readonly IIdentificationSystem _identificationSystem;
        private readonly ISaveLoadSystem _saveLoadService;
        private readonly SignalBus _signalBus;

        private Dictionary<int, InventorySystemData> _inventorySystems = new();

        public bool CreateInventory(InventoryType inventoryType, out int inventoryID)
        {
            inventoryID = -1;
            if (_inventoriesConfig.TryGetInventoryConfig(inventoryType, out InventoryConfig inventoryConfig))
            {
                InventorySystem inventorySystem = new InventorySystem(inventoryConfig);
                if (_identificationSystem.Registry(out int id))
                {
                    InitializeLoadInventory(inventoryType, inventorySystem);

                    _inventorySystems.Add(id, new InventorySystemData
                    {
                        InventoryType = inventoryType,
                        InventorySystem = inventorySystem
                    });
                    inventoryID = id;
                }
            }

            return inventoryID != -1;
        }

        public void OpenInventoryWithID(InventoryIdentificationData identificationData)
        {
            if (_inventorySystems.TryGetValue(identificationData.ID, out InventorySystemData inventorySystem))
            {
                _signalBus.Fire(new OpenInventorySignal
                {
                    IsOpen = true,
                    InventorySystemData = inventorySystem
                });
            }
        }

        public void CloseInventory(InventoryIdentificationData identificationData)
        {
            InitializeSaveInventory(identificationData);
            if (_inventorySystems.TryGetValue(identificationData.ID, out InventorySystemData inventorySystem))
            {
                _signalBus.Fire(new OpenInventorySignal
                {
                    IsOpen = false,
                    InventorySystemData = inventorySystem
                });
            }
        }

        private void InitializeLoadInventory(InventoryType inventoryType, InventorySystem inventorySystem)
        {
            if (_saveLoadService.TryLoadData(inventoryType.ToString(), out InventorySaveData inventorySaveData) && inventorySaveData != null)
            {
                foreach (PlacementItem item in inventorySaveData.Items)
                {
                    inventorySystem.TryRestorePlacement(item);
                }
            }
        }

        private void InitializeSaveInventory(InventoryIdentificationData identificationData)
        {
            if (_inventorySystems.TryGetValue(identificationData.ID, out InventorySystemData inventorySystem))
            {
                InventorySaveData inventorySaveData = new();
                foreach (PlacementItem item in inventorySystem.InventorySystem.PlacementItems)
                {
                    inventorySaveData.Items.Add(item);
                }

                _saveLoadService.SaveData(identificationData.InventoryType.ToString(), inventorySaveData);
            }
        }
    }

    public interface IInventoryManager
    {
        public bool CreateInventory(InventoryType inventoryType, out int inventoryID);
        public void OpenInventoryWithID(InventoryIdentificationData identificationData);
        public void CloseInventory(InventoryIdentificationData identificationData);
    }

    [Serializable]
    public class InventorySystemData
    {
        public bool IsOpen;
        public InventoryType InventoryType;
        public InventorySystem InventorySystem;
    }

    [Serializable]
    public struct InventoryIdentificationData
    {
        public InventoryType InventoryType;
        public int ID;
    }
}