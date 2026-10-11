namespace GridSystem.Integrations.Zenject
{
    using global::Zenject;
    using Runtime.Composition;

    /// <summary>
    /// Runs grid entry point in Zenject initialization phase.
    /// </summary>
    public sealed class ZenjectGridEntryPoint : IInitializable
    {
        public ZenjectGridEntryPoint(GridEntryPoint entryPoint)
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
