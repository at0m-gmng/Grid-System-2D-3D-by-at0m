namespace GridSystem.Samples.Editor
{
    using Runtime.Composition;

    /// <summary>
    /// Adds a DI container to the generated sample scene.
    /// </summary>
    public interface ISampleContainerSetup
    {
        /// <summary>
        /// Container name used in the scene file name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates project wide assets required by the container.
        /// </summary>
        public void PrepareProject();

        /// <summary>
        /// Creates container objects in the active scene and installs the context.
        /// </summary>
        public void Setup(GridSystemContext context);
    }
}
