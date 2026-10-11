namespace GridSystem.Samples
{
    using Bootstrap;
    using Description;
    using Runtime.Composition;
    using UnityEngine;

    /// <summary>
    /// Registers sample services. Add it to <see cref="GridSystemContext"/> modules.
    /// </summary>
    public sealed class SampleModule : GridModule
    {
        [SerializeField]
        private SampleSceneData _data;

        [SerializeField]
        private ItemDescriptionView _description;

        /// <summary>
        /// Adds sample bindings.
        /// </summary>
        public override void Install(IGridRegistrar registrar)
        {
            registrar.RegisterInstance(_data);
            registrar.RegisterInstance(_description);
            registrar.Register<SampleWindowPresenter>();
            registrar.Register<SampleAutoSave>();
            registrar.Register<SampleBootstrap>();
            registrar.Register<ItemDescriptionPresenter>();
        }
    }
}
