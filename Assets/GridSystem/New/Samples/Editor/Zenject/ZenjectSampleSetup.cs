namespace GridSystem.Samples.Editor.Zenject
{
    using global::Zenject;
    using Integrations.Zenject;
    using Runtime.Composition;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Adds Zenject scene context with the grid system installer.
    /// </summary>
    public sealed class ZenjectSampleSetup : ISampleContainerSetup
    {
        private const string NAME = "Zenject";
        private const string CONTEXT_NAME = "SceneContext";
        private const string CONTEXT_FIELD = "_context";

        /// <summary>
        /// Container name.
        /// </summary>
        public string Name => NAME;

        /// <summary>
        /// Zenject needs no project assets.
        /// </summary>
        public void PrepareProject()
        {
        }

        /// <summary>
        /// Creates scene context with the installer.
        /// </summary>
        public void Setup(GridSystemContext context)
        {
            GameObject target = new GameObject(CONTEXT_NAME);
            SceneContext sceneContext = target.AddComponent<SceneContext>();
            ZenjectGridInstaller installer = target.AddComponent<ZenjectGridInstaller>();
            new SerializedObject(installer).SetObject(CONTEXT_FIELD, context).Apply();
            sceneContext.Installers = new MonoInstaller[] { installer };
        }

        [MenuItem(SampleGenerator.SCENE_MENU + NAME)]
        private static void CreateScene() => SampleGenerator.CreateScene(new ZenjectSampleSetup());
    }
}
