namespace GridSystem.Samples.Bootstrap
{
    using Runtime.Composition;
    using Runtime.Surfaces;
    using UnityEngine;

    /// <summary>
    /// Spawns the sample content through the container on first use and keeps the created instances.
    /// Spawning is lazy, so consumers do not depend on initialization order.
    /// </summary>
    public sealed class SampleContent
    {
        public SampleContent(IGridObjectFactory factory, SampleSceneConfig config, Camera camera)
        {
            _factory = factory;
            _config = config;
            _camera = camera;
        }

        private readonly IGridObjectFactory _factory;
        private readonly SampleSceneConfig _config;
        private readonly Camera _camera;
        private SampleSceneRoot _ui;
        private GridSurface _world;

        /// <summary>
        /// Spawned UI with its surfaces, windows and tooltip.
        /// </summary>
        public SampleSceneRoot Ui
        {
            get
            {
                Spawn();
                return _ui;
            }
        }

        /// <summary>
        /// Spawned world grid surface.
        /// </summary>
        public GridSurface World
        {
            get
            {
                Spawn();
                return _world;
            }
        }

        private void Spawn()
        {
            if (_ui == null)
            {
                _ui = _factory.Create(_config.UiPrefab, null);
                _world = _factory.Create(_config.WorldPrefab, null);
                if (_world is WorldGridSurface worldSurface)
                {
                    worldSurface.SetCamera(_camera);
                }
            }
        }
    }
}
