namespace GridSystem.Integrations.Reflex
{
    using global::Reflex.Core;
    using global::Reflex.Injectors;
    using Runtime.Composition;
    using UnityEngine;

    /// <summary>
    /// Reflex implementation of the prefab factory. Reflex has no instantiation helper,
    /// so the hierarchy is created first and injected afterwards.
    /// </summary>
    public sealed class ReflexGridObjectFactory : IGridObjectFactory
    {
        public ReflexGridObjectFactory(Container container)
        {
            _container = container;
        }

        private readonly Container _container;

        /// <summary>
        /// Instantiates the prefab under the parent and injects the created hierarchy.
        /// </summary>
        public TComponent Create<TComponent>(TComponent prefab, Transform parent) where TComponent : Component
        {
            TComponent instance = Object.Instantiate(prefab, parent);
            GameObjectInjector.InjectRecursive(instance.gameObject, _container);
            return instance;
        }
    }
}
