namespace GridSystem.Samples
{
    using Bootstrap;
    using Runtime.Composition;
    using Tooltip;
    using UnityEngine;

    /// <summary>
    /// Registers sample services. Add it to <see cref="GridSystemContext"/> modules.
    /// </summary>
    public sealed class SampleModule : GridModule
    {
        [SerializeField]
        private SampleSceneData _data;

        [SerializeField]
        private ItemTooltipView _tooltip;

        /// <summary>
        /// Adds sample bindings.
        /// </summary>
        public override void Install(IGridRegistrar registrar)
        {
            registrar.RegisterInstance(_data);
            registrar.RegisterInstance(_tooltip);
            registrar.Register<SampleWindowPresenter>();
            registrar.Register<SampleAutoSave>();
            registrar.Register<SampleBootstrap>();
            registrar.Register<ItemTooltipPresenter>();
        }
    }
}
