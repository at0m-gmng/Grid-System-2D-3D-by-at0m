namespace GridSystem.Samples
{
    using Bootstrap;
    using Runtime.Composition;
    using Tooltip;
    using UnityEngine;

    /// <summary>
    /// Registers sample services. Add it to <see cref="GridSystemContext"/> modules.
    /// Everything except the camera comes from the config asset, so container scenes share one setup.
    /// </summary>
    public sealed class SampleModule : GridModule
    {
        [SerializeField]
        private SampleSceneConfig _config;

        [SerializeField]
        private Camera _camera;

        /// <summary>
        /// Adds sample bindings.
        /// </summary>
        public override void Install(IGridRegistrar registrar)
        {
            registrar.RegisterInstance(_config);
            registrar.RegisterInstance(_camera);
            registrar.Register<SampleContent>();
            registrar.Register<SampleWindowPresenter>();
            registrar.Register<SampleAutoSave>();
            registrar.Register<SampleBootstrap>();
            registrar.Register<ItemTooltipPresenter>();
        }
    }
}
