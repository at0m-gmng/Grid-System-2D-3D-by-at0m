namespace GridSystem.Integrations.VContainer
{
    using global::VContainer.Unity;
    using Runtime.Composition;

    /// <summary>
    /// Runs grid entry point in VContainer initialization phase.
    /// </summary>
    public sealed class VContainerGridEntryPoint : IInitializable
    {
        public VContainerGridEntryPoint(GridEntryPoint entryPoint)
        {
            _entryPoint = entryPoint;
        }

        private readonly GridEntryPoint _entryPoint;

        /// <summary>
        /// Initializes grid services.
        /// </summary>
        public void Initialize() => _entryPoint.Initialize();
    }
}
