namespace GridSystem.Integrations.Zenject
{
    using global::Zenject;
    using Runtime.Composition;

    /// <summary>
    /// Installs grid system into any Zenject container.
    /// </summary>
    public static class ZenjectGridExtensions
    {
        /// <summary>
        /// Registers grid system and modules of the context.
        /// </summary>
        public static void InstallGridSystem(this DiContainer container, GridSystemContext context)
        {
            context.Install(new ZenjectGridRegistrar(container));
            container.BindInterfacesTo<ZenjectGridEntryPoint>().AsSingle();
        }
    }
}
