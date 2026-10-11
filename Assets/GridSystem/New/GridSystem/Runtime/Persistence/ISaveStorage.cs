namespace GridSystem.Runtime.Persistence
{
    /// <summary>
    /// Key-value text storage.
    /// </summary>
    public interface ISaveStorage
    {
        /// <summary>
        /// Reads stored value.
        /// </summary>
        public bool TryRead(string key, out string value);

        /// <summary>
        /// Writes value.
        /// </summary>
        public void Write(string key, string value);

        /// <summary>
        /// Removes value.
        /// </summary>
        public void Delete(string key);
    }
}
