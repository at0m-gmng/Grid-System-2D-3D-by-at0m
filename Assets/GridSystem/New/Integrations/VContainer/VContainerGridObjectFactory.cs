namespace GridSystem.Integrations.VContainer
{
    using global::VContainer;
    using global::VContainer.Unity;
    using Runtime.Composition;
    using UnityEngine;

    /// <summary>
    /// VContainer implementation of the prefab factory.
    /// </summary>
    public sealed class VContainerGridObjectFactory : IGridObjectFactory
    {
        public VContainerGridObjectFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        private readonly IObjectResolver _resolver;

        /// <summary>
        /// Instantiates the prefab under the parent and injects the created hierarchy.
        /// </summary>
        public TComponent Create<TComponent>(TComponent prefab, Transform parent) where TComponent : Component
            => _resolver.Instantiate(prefab, parent);
    }
}
