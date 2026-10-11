namespace GridSystem.Runtime.Interaction.Drop
{
    using Core.Items;
    using Inventories;
    using MessagePipe;
    using Merge;
    using Messaging;

    /// <summary>
    /// Replaces dragged and target items with the merge result at the target position.
    /// </summary>
    public sealed class MergeDropHandler : IItemDropHandler
    {
        public MergeDropHandler(IMergeRules rules, IGridItemFactory factory, IPublisher<ItemsMergedMessage> publisher)
        {
            _rules = rules;
            _factory = factory;
            _publisher = publisher;
        }

        private readonly IMergeRules _rules;
        private readonly IGridItemFactory _factory;
        private readonly IPublisher<ItemsMergedMessage> _publisher;

        /// <summary>
        /// Handler order.
        /// </summary>
        public int Order => DropHandlerOrder.MERGE;

        /// <summary>
        /// Checks recipe and that the result fits into cells of both ingredients.
        /// </summary>
        public bool CanHandle(in DropContext context)
        {
            GridItem target = context.TargetItem;
            bool result = target != null
                && target != context.Item
                && _rules.TryGetResult(context.Item.Definition, target.Definition, out IGridItemDefinition definition)
                && context.Target.CanPlace(definition, target.Origin, target.Rotation, context.Item.Id, target.Id);
            return result;
        }

        /// <summary>
        /// Removes ingredients and places the result.
        /// </summary>
        public void Handle(in DropContext context)
        {
            GridItem target = context.TargetItem;
            _rules.TryGetResult(context.Item.Definition, target.Definition, out IGridItemDefinition definition);
            GridItem result = _factory.Create(definition);
            context.Source.TryRemove(context.Item);
            context.Target.TryRemove(target);
            context.Target.TryAdd(result, target.Origin, target.Rotation);
            _publisher.Publish(new ItemsMergedMessage(context.Target, context.Item, target, result));
        }
    }
}
