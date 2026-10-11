namespace GridSystem.Integrations.Reflex
{
    using System;
    using global::Reflex.Core;
    using global::Reflex.Enums;
    using Runtime.Composition;

    /// <summary>
    /// Reflex implementation of the registration facade.
    /// </summary>
    public sealed class ReflexGridRegistrar : IGridRegistrar
    {
        public ReflexGridRegistrar(ContainerBuilder builder)
        {
            _builder = builder;
        }

        private readonly ContainerBuilder _builder;

        /// <summary>
        /// Registers lazy singleton bound to itself and all implemented interfaces.
        /// </summary>
        public void Register<TImplementation>() where TImplementation : class
            => _builder.RegisterType(typeof(TImplementation), GetContracts(typeof(TImplementation)), Lifetime.Singleton, Resolution.Lazy);

        /// <summary>
        /// Registers existing instance bound only to the contract type.
        /// </summary>
        public void RegisterInstance<TContract>(TContract instance) where TContract : class => _builder.RegisterValue(instance, new[] { typeof(TContract) });

        private static Type[] GetContracts(Type type)
        {
            Type[] interfaces = type.GetInterfaces();
            Type[] contracts = new Type[interfaces.Length + 1];
            contracts[0] = type;
            Array.Copy(interfaces, 0, contracts, 1, interfaces.Length);
            return contracts;
        }
    }
}
