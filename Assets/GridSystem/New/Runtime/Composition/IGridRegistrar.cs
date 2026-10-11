namespace GridSystem.Runtime.Composition
{
    /// <summary>
    /// DI agnostic registration facade implemented by every container adapter.
    /// </summary>
    public interface IGridRegistrar
    {
        /// <summary>
        /// Registers lazy singleton bound to itself and all implemented interfaces.
        /// </summary>
        public void Register<TImplementation>() where TImplementation : class;

        /// <summary>
        /// Registers existing instance bound only to the contract type.
        /// </summary>
        public void RegisterInstance<TContract>(TContract instance) where TContract : class;
    }
}
