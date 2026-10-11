namespace GridSystem.Integrations.Reflex
{
    using Runtime.Composition;

    /// <summary>
    /// Eagerly resolved by Reflex after the container is built, runs grid entry point.
    /// </summary>
    public sealed class ReflexGridEntryPoint
    {
        public ReflexGridEntryPoint(GridEntryPoint entryPoint) => entryPoint.Initialize();
    }
}
