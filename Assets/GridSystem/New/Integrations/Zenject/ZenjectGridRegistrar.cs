namespace GridSystem.Integrations.Zenject
{
    using global::Zenject;
    using Runtime.Composition;

    /// <summary>
    /// Zenject implementation of the registration facade.
    /// </summary>
    public sealed class ZenjectGridRegistrar : IGridRegistrar
    {
        public ZenjectGridRegistrar(DiContainer container)
        {
            _container = container;
        }

        private readonly DiContainer _container;

        /// <summary>
        /// Registers lazy singleton bound to itself and all implemented interfaces.
        /// </summary>
        public void Register<TImplementation>() where TImplementation : class => _container.BindInterfacesAndSelfTo<TImplementation>().AsSingle();

        /// <summary>
        /// Registers existing instance bound only to the contract type.
        /// </summary>
        public void RegisterInstance<TContract>(TContract instance) where TContract : class => _container.Bind<TContract>().FromInstance(instance).AsSingle();
    }
}
