namespace GridSystem.Installers
{
    using DestroySystem;
    using DragDropSystem;
    using IdentificationSystem;
    using InputSystem;
    using InventorySystem.Inventory.Configs;
    using InventorySystem.Inventory.Core;
    using InventorySystem.Inventory.Data;
    using MergeSystem;
    using OverlappedSystem;
    using OverlappedSystem.Data;
    using SaveSystem;
    using Signals;
    using UnityEngine;
    using Zenject;

    public sealed class GameInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField]
        private InputConfig _inputConfig;

        [SerializeField]
        private InventoryConfig _inventoryConfig;

        [SerializeField]
        private MergeConfig _mergeConfig;

        [SerializeField]
        private SelectableItemConfig _selectableItemConfig;

        [SerializeField]
        private WorldOverlapConfig _worldOverlapConfig;

        [SerializeField]
        private InventoriesConfig _inventoriesConfig;

        [SerializeField]
        private UIItemsData _uiItemsData;

        [SerializeField]
        private WorldItemsData _worldItemsData;

        public override void InstallBindings()
        {
            InstallConfigs();
            InstallSignals();
            InstallControllers();
        }

        private void InstallConfigs()
        {
            Container.BindInstance(_inventoryConfig).AsSingle().IfNotBound();
            Container.BindInstance(_mergeConfig).AsSingle().IfNotBound();
            Container.BindInstance(_selectableItemConfig).AsSingle().IfNotBound();
            Container.BindInstance(_uiItemsData).AsSingle().IfNotBound();
            Container.BindInstance(_worldItemsData).AsSingle().IfNotBound();
        }

        private void InstallControllers()
        {
            Container.BindInterfacesTo<SaveLoadSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputSystem>().AsSingle().WithArguments(_inputConfig);

            Container.Bind<UIOverlapSystem>().AsSingle();
            Container.Bind<WorldOverlapSystem>().AsSingle().WithArguments(_worldOverlapConfig);

            Container.BindInterfacesAndSelfTo<ItemsIdentificationSystem>().AsSingle();
            Container.BindInterfacesTo<MergeHandler>().AsSingle().WithArguments(_mergeConfig);
            Container.BindInterfacesTo<DestroySystem>().AsSingle();

            Container.BindInterfacesAndSelfTo<UIDragDropSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<WorldDragDropSystem>().AsSingle();

            Container.BindInterfacesAndSelfTo<InventorySystem>().AsSingle().WithArguments(_inventoryConfig);

            Container.BindInterfacesTo<InventoryManager>().AsSingle().WithArguments(_inventoriesConfig);
        }

        private void InstallSignals()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<ItemDataViewSignal>();
            Container.DeclareSignal<DragDropStateSignal>();
            Container.DeclareSignal<CreatedItemSignal>();
            Container.DeclareSignal<SelectedItemSignal>();
            Container.DeclareSignal<DataSelectedForSpawnSignal>();
            Container.DeclareSignal<ItemMergedSignal>();
            Container.DeclareSignal<MergedResultSignal>();
            Container.DeclareSignal<OpenInventorySignal>();
            Container.DeclareSignal<InventoryViewSpawnSignal>();
        }
    }
}