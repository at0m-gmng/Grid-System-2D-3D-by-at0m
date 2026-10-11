namespace GridSystem.Integrations.Zenject
{
    using global::Zenject;
    using Runtime.Composition;
    using UnityEngine;

    /// <summary>
    /// Ready to use Zenject installer. Add it to a SceneContext.
    /// </summary>
    public sealed class ZenjectGridInstaller : MonoInstaller
    {
        [SerializeField]
        private GridSystemContext _context;

        /// <summary>
        /// Installs grid system bindings.
        /// </summary>
        public override void InstallBindings() => Container.InstallGridSystem(_context);
    }
}
