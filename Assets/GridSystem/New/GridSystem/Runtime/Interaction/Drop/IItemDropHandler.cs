namespace GridSystem.Runtime.Interaction.Drop
{
    /// <summary>
    /// Drop strategy. The first handler accepting the context in ascending order handles the drop.
    /// </summary>
    public interface IItemDropHandler
    {
        /// <summary>
        /// Lower values are asked first.
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// Checks the drop without side effects. Called every frame while dragging.
        /// </summary>
        public bool CanHandle(in DropContext context);

        /// <summary>
        /// Performs the drop accepted by <see cref="CanHandle"/>.
        /// </summary>
        public void Handle(in DropContext context);
    }
}
