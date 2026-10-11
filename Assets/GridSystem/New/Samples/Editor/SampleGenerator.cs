namespace GridSystem.Samples.Editor
{
    using Runtime.Composition;
    using Runtime.Pointer;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Generates sample content and scenes.
    /// </summary>
    public static class SampleGenerator
    {
        private const string MENU_ROOT = "Tools/Grid System/Samples/";
        private const string CONTENT_MENU = MENU_ROOT + "Create Content";
        private const string SCENE_PREFIX = "GridSystem_";

        /// <summary>
        /// Menu path prefix of scene generators.
        /// </summary>
        public const string SCENE_MENU = MENU_ROOT + "Create Scene/";

        /// <summary>
        /// Creates or updates assets and prefabs.
        /// </summary>
        [MenuItem(CONTENT_MENU)]
        public static void CreateContentMenu() => CreateContent();

        /// <summary>
        /// Creates or updates content and creates a new scene with the container.
        /// </summary>
        public static void CreateScene(ISampleContainerSetup container)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                SampleContent content = CreateContent();
                container.PrepareProject();
                GridSystemContext context = new SampleSceneBuilder().Build(content);
                container.Setup(context);
                SampleAssets.EnsureFolder(SamplePaths.SCENES);
                EditorSceneManager.SaveScene(scene, SamplePaths.Combine(SamplePaths.SCENES, SCENE_PREFIX + container.Name, SamplePaths.SCENE_EXTENSION));
            }
        }

        private static SampleContent CreateContent()
        {
            SampleContent content = new SampleContent();
            PointerInputConfig input = new SampleInputBuilder().Build(content);
            new SampleDataBuilder().Build(content, input);
            new SamplePrefabBuilder().Build(content);
            AssetDatabase.SaveAssets();
            return content;
        }
    }
}
