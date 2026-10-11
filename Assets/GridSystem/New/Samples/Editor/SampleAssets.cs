namespace GridSystem.Samples.Editor
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Asset database helpers of the sample generator.
    /// </summary>
    public static class SampleAssets
    {
        /// <summary>
        /// Creates the folder with all missing parents.
        /// </summary>
        public static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                int separator = path.LastIndexOf(SamplePaths.SEPARATOR[0]);
                string parent = path.Substring(0, separator);
                EnsureFolder(parent);
                AssetDatabase.CreateFolder(parent, path.Substring(separator + 1));
            }
        }

        /// <summary>
        /// Loads the asset or creates a new one at the path.
        /// </summary>
        public static T LoadOrCreate<T>(string folder, string name) where T : ScriptableObject
        {
            string path = SamplePaths.Combine(folder, name, SamplePaths.ASSET_EXTENSION);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                EnsureFolder(folder);
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
            }

            return asset;
        }

        /// <summary>
        /// Saves the temporary object as prefab, replacing an existing one, destroys the object
        /// and returns the root component of the prefab asset.
        /// </summary>
        public static T SavePrefab<T>(GameObject root, string folder) where T : Component
        {
            EnsureFolder(folder);
            string path = SamplePaths.Combine(folder, root.name, SamplePaths.PREFAB_EXTENSION);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        /// <summary>
        /// Loads an asset that must exist.
        /// </summary>
        public static T Load<T>(string path) where T : Object => AssetDatabase.LoadAssetAtPath<T>(path);
    }
}
