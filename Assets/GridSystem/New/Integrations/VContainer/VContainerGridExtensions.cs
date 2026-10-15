namespace GridSystem.Integrations.VContainer
{
    using global::VContainer;
    using global::VContainer.Unity;
    using Runtime.Composition;

    /// <summary>
    /// Installs grid system into any VContainer builder.
    /// </summary>
    public static class VContainerGridExtensions
    {
        /// <summary>
        /// Registers grid system and modules of the context.
        /// </summary>
        public static void InstallGridSystem(this IContainerBuilder builder, GridSystemContext context)
        {
            builder.Register<VContainerGridObjectFactory>(Lifetime.Singleton).As<IGridObjectFactory>();
            context.Install(new VContainerGridRegistrar(builder));
            builder.RegisterEntryPoint<VContainerGridEntryPoint>();
        }
    }
}
