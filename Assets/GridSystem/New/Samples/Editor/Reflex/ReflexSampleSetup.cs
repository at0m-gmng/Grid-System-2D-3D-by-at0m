namespace GridSystem.Samples.Editor.Reflex
{
    using System;
    using global::Reflex.Core;
    using Integrations.Reflex;
    using Runtime.Composition;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Adds Reflex scene scope with the grid system installer.
    /// </summary>
    public sealed class ReflexSampleSetup : ISampleContainerSetup
    {
        private const string NAME = "Reflex";
        private const string SCOPE_NAME = "SceneScope";
        private const string RESOURCES = "Assets/Resources";
        private const string SETTINGS_NAME = "ReflexSettings";
        private const string SETTINGS_SEARCH = "t:" + SETTINGS_NAME;
        private const string SETTINGS_TYPE = "Reflex.Configuration.ReflexSettings";
        private const string CONTEXT_FIELD = "_context";

        /// <summary>
        /// Container name.
        /// </summary>
        public string Name => NAME;

        /// <summary>
        /// Creates ReflexSettings in Resources when the project has none.
        /// </summary>
        public void PrepareProject()
        {
            if (AssetDatabase.FindAssets(SETTINGS_SEARCH).Length == 0)
            {
                SampleAssets.EnsureFolder(RESOURCES);
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(FindSettingsType()),
                    SamplePaths.Combine(RESOURCES, SETTINGS_NAME, SamplePaths.ASSET_EXTENSION));
            }
        }

        /// <summary>
        /// Creates scene scope with the installer.
        /// </summary>
        public void Setup(GridSystemContext context)
        {
            GameObject scope = new GameObject(SCOPE_NAME);
            scope.AddComponent<ContainerScope>();
            ReflexGridInstaller installer = scope.AddComponent<ReflexGridInstaller>();
            new SerializedObject(installer).SetObject(CONTEXT_FIELD, context).Apply();
        }

        [MenuItem(SampleGenerator.SCENE_MENU + NAME)]
        private static void CreateScene() => SampleGenerator.CreateScene(new ReflexSampleSetup());

        private static Type FindSettingsType()
        {
            Type result = null;
            foreach (Type type in TypeCache.GetTypesDerivedFrom<ScriptableObject>())
            {
                if (type.FullName == SETTINGS_TYPE)
                {
                    result = type;
                }
            }

            return result;
        }
    }
}
