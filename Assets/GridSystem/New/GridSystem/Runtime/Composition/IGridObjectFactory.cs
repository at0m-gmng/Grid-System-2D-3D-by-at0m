namespace GridSystem.Runtime.Composition
{
    using UnityEngine;

    /// <summary>
    /// DI agnostic prefab factory implemented by every container adapter.
    /// Objects created through it get their dependencies injected by the container,
    /// so content spawned at runtime behaves like content placed in the scene.
    /// </summary>
    public interface IGridObjectFactory
    {
        /// <summary>
        /// Instantiates the prefab under the parent and injects the created hierarchy.
        /// </summary>
        public TComponent Create<TComponent>(TComponent prefab, Transform parent) where TComponent : Component;
    }
}
