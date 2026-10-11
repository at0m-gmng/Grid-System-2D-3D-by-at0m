namespace GridSystem.Runtime.Composition
{
    using Interaction;
    using Interaction.Drop;
    using Inventories;
    using Messaging;
    using Persistence;
    using Pointer;
    using Settings;
    using Surfaces;
    using UnityEngine;
    using Views;

    /// <summary>
    /// Single description of grid system bindings shared by all container adapters.
    /// </summary>
    public static class GridSystemComposition
    {
        /// <summary>
        /// Registers configs, message brokers, services and entry point.
        /// </summary>
        public static void Install(IGridRegistrar registrar, GridSystemSettings settings, Transform poolRoot)
        {
            registrar.RegisterInstance(settings);
            registrar.RegisterInstance(settings.Items);
            registrar.RegisterInstance(settings.MergeRules);
            registrar.RegisterInstance(settings.Input);
            registrar.RegisterInstance(new ItemViewPool(poolRoot));
            GridMessageBus.Install(registrar);

            registrar.Register<InputSystemPointerInput>();
            registrar.Register<PlayerPrefsSaveStorage>();
            registrar.Register<GridItemFactory>();
            registrar.Register<InventoryService>();
            registrar.Register<InventoryPersistence>();
            registrar.Register<GridSurfacePresenterFactory>();
            registrar.Register<SurfaceService>();
            registrar.Register<MergeDropHandler>();
            registrar.Register<PlaceDropHandler>();
            registrar.Register<ItemDragService>();
            registrar.Register<ItemHoverService>();
            registrar.Register<GridEntryPoint>();
        }
    }
}
