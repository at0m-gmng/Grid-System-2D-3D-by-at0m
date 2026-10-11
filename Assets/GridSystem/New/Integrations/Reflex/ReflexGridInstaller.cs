namespace GridSystem.Integrations.Reflex
{
    using global::Reflex.Core;
    using Runtime.Composition;
    using UnityEngine;

    /// <summary>
    /// Ready to use Reflex installer. Place it on the scene ContainerScope object or its child.
    /// </summary>
    public sealed class ReflexGridInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField]
        private GridSystemContext _context;

        /// <summary>
        /// Installs grid system bindings.
        /// </summary>
        public void InstallBindings(ContainerBuilder containerBuilder) => containerBuilder.InstallGridSystem(_context);
    }
}
