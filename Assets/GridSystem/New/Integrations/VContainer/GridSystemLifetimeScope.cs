namespace GridSystem.Integrations.VContainer
{
    using global::VContainer;
    using global::VContainer.Unity;
    using Runtime.Composition;
    using UnityEngine;

    /// <summary>
    /// Ready to use VContainer scope. Use <see cref="VContainerGridExtensions"/> in your own scope instead if needed.
    /// </summary>
    public sealed class GridSystemLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private GridSystemContext _context;

        protected override void Configure(IContainerBuilder builder) => builder.InstallGridSystem(_context);
    }
}
