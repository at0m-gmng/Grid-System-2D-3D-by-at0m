namespace GridSystem.Runtime.Composition
{
    /// <summary>
    /// Entry point invoked by the container adapter after the container is built.
    /// </summary>
    public interface IGridInitializable
    {
        /// <summary>
        /// Starts the service.
        /// </summary>
        public void Initialize();
    }
}
