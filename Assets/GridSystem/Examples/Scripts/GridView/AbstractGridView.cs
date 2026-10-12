namespace GridSystem.GridView
{
    using System.Collections.Generic;
    using CreatedSystem.Data;
    using Cysharp.Threading.Tasks;
    using DragDropSystem.View;
    using InventoryPresenters;
    using InventorySystem.Inventory.Core;
    using InventorySystem.Inventory.Data;
    using InventorySystem.Inventory.Enums;
    using Signals;
    using UnityEngine;
    using Zenject;

    public abstract class AbstractGridView<T, D> : BaseGridView
        where T : Transform
        where D : IInventoryPresenter
    {
        [Inject]
        protected virtual void Construct(SignalBus signalBus)
        {
            this.signalBus = signalBus;
            this.signalBus.Subscribe<OpenInventorySignal>(InitializeInventory);
        }

        protected SignalBus signalBus;


        [SerializeField]
        protected T itemParent;

        [SerializeField]
        protected T outsideParent;

        [SerializeField]
        protected List<InventoryType> inventoryType;

        [SerializeField]
        protected T cellPrefab;

        protected List<List<T>> cellObjects = new();
        protected SpawnData _spawnData;

        protected virtual void InitializeInventory(OpenInventorySignal signal)
        {
            if (inventoryType.Contains(signal.InventorySystemData.InventoryType))
            {
                if (!signal.IsOpen)
                {
                    CloseInventory();
                    return;
                }

                OpenInventory(signal.InventorySystemData);
            }
        }

        protected virtual async void OpenInventory(InventorySystemData inventorySystem)
        {
            _spawnData = new SpawnData
            {
                InventoryPresenter = null,
                PlacementItems = new List<PlacementItem>(inventorySystem.InventorySystem.PlacementItems)
            };

#if UNITY_EDITOR
            CreateDefaultItems(inventorySystem);
#endif
            Initialize(
                inventorySystem.InventorySystem.InventoryConfig.Rows,
                inventorySystem.InventorySystem.InventoryConfig.Columns
            );
            presenter = CreatePresenter(inventorySystem);
            _spawnData.InventoryPresenter = presenter;

            await UniTask.DelayFrame(1);
            signalBus.Fire(new InventoryViewSpawnSignal
            {
                SpawnData = _spawnData
            });
        }

        protected virtual void CloseInventory()
        {
            IPoolableDraggableObject[] draggableObjects = GetComponentsInChildren<IPoolableDraggableObject>();
            for (int i = 0; i < draggableObjects.Length; i++)
            {
                draggableObjects[i].ReturnToPool();
            }
        }

        protected abstract void Initialize(int rows, int columns);

        protected abstract void UpdateGrid(int rows, int cols);

        protected virtual void CreateDefaultItems(InventorySystemData inventorySystem)
        {
        }

        protected abstract D CreatePresenter(InventorySystemData inventorySystem);
    }
}