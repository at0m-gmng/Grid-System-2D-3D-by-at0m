namespace GridSystem.Integrations.Zenject
{
    using global::Zenject;
    using Runtime.Composition;
    using UnityEngine;

    /// <summary>
    /// Zenject implementation of the prefab factory.
    /// </summary>
    public sealed class ZenjectGridObjectFactory : IGridObjectFactory
    {
        public ZenjectGridObjectFactory(DiContainer container)
        {
            _container = container;
        }

        private readonly DiContainer _container;

        /// <summary>
        /// Instantiates the prefab under the parent and injects the created hierarchy.
        /// </summary>
        public TComponent Create<TComponent>(TComponent prefab, Transform parent) where TComponent : Component
            => _container.InstantiatePrefabForComponent<TComponent>(prefab, parent);
    }
}
