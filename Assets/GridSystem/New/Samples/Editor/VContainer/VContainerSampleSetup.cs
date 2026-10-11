namespace GridSystem.Samples.Editor.VContainer
{
    using Integrations.VContainer;
    using Runtime.Composition;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Adds VContainer lifetime scope with the grid system.
    /// </summary>
    public sealed class VContainerSampleSetup : ISampleContainerSetup
    {
        private const string NAME = "VContainer";
        private const string SCOPE_NAME = "LifetimeScope";
        private const string CONTEXT_FIELD = "_context";

        /// <summary>
        /// Container name.
        /// </summary>
        public string Name => NAME;

        /// <summary>
        /// VContainer needs no project assets.
        /// </summary>
        public void PrepareProject()
        {
        }

        /// <summary>
        /// Creates lifetime scope with the grid system.
        /// </summary>
        public void Setup(GridSystemContext context)
        {
            GridSystemLifetimeScope scope = new GameObject(SCOPE_NAME).AddComponent<GridSystemLifetimeScope>();
            new SerializedObject(scope).SetObject(CONTEXT_FIELD, context).Apply();
        }

        [MenuItem(SampleGenerator.SCENE_MENU + NAME)]
        private static void CreateScene() => SampleGenerator.CreateScene(new VContainerSampleSetup());
    }
}
