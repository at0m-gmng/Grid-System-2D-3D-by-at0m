namespace GridSystem.Integrations.VContainer
{
    using global::VContainer;
    using Runtime.Composition;

    /// <summary>
    /// VContainer implementation of the registration facade.
    /// </summary>
    public sealed class VContainerGridRegistrar : IGridRegistrar
    {
        public VContainerGridRegistrar(IContainerBuilder builder)
        {
            _builder = builder;
        }

        private readonly IContainerBuilder _builder;

        /// <summary>
        /// Registers lazy singleton bound to itself and all implemented interfaces.
        /// </summary>
        public void Register<TImplementation>() where TImplementation : class
            => _builder.Register<TImplementation>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

        /// <summary>
        /// Registers existing instance bound only to the contract type.
        /// </summary>
        public void RegisterInstance<TContract>(TContract instance) where TContract : class => _builder.RegisterInstance(instance);
    }
}
