namespace GridSystem.Runtime.Persistence
{
    using UnityEngine;

    /// <summary>
    /// PlayerPrefs based storage.
    /// </summary>
    public sealed class PlayerPrefsSaveStorage : ISaveStorage
    {
        private const string KEY_PREFIX = "GridSystem.";

        /// <summary>
        /// Reads stored value.
        /// </summary>
        public bool TryRead(string key, out string value)
        {
            string fullKey = KEY_PREFIX + key;
            bool result = PlayerPrefs.HasKey(fullKey);
            value = result ? PlayerPrefs.GetString(fullKey) : null;
            return result;
        }

        /// <summary>
        /// Writes value.
        /// </summary>
        public void Write(string key, string value)
        {
            PlayerPrefs.SetString(KEY_PREFIX + key, value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Removes value.
        /// </summary>
        public void Delete(string key) => PlayerPrefs.DeleteKey(KEY_PREFIX + key);
    }
}
