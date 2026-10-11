namespace GridSystem.Runtime.Interaction.Drop
{
    /// <summary>
    /// Moves the item inside the inventory or transfers it to another inventory.
    /// </summary>
    public sealed class PlaceDropHandler : IItemDropHandler
    {
        /// <summary>
        /// Handler order.
        /// </summary>
        public int Order => DropHandlerOrder.PLACE;

        /// <summary>
        /// Checks that the item fits ignoring its own cells.
        /// </summary>
        public bool CanHandle(in DropContext context) => context.Target.CanPlace(context.Item, context.Cell, context.Rotation);

        /// <summary>
        /// Moves or transfers the item.
        /// </summary>
        public void Handle(in DropContext context) => context.Source.TryTransfer(context.Item, context.Target, context.Cell, context.Rotation);
    }
}
