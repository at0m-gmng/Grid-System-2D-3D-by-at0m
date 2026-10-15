namespace GridSystem.Integrations.Reflex
{
    using global::Reflex.Core;
    using global::Reflex.Enums;
    using Runtime.Composition;

    /// <summary>
    /// Installs grid system into any Reflex container builder.
    /// </summary>
    public static class ReflexGridExtensions
    {
        /// <summary>
        /// Registers grid system and modules of the context.
        /// </summary>
        public static void InstallGridSystem(this ContainerBuilder builder, GridSystemContext context)
        {
            builder.RegisterType(typeof(ReflexGridObjectFactory), new[] { typeof(IGridObjectFactory) }, Lifetime.Singleton, Resolution.Lazy);
            context.Install(new ReflexGridRegistrar(builder));
            builder.RegisterType(typeof(ReflexGridEntryPoint), new[] { typeof(ReflexGridEntryPoint) }, Lifetime.Singleton, Resolution.Eager);
        }
    }
}
