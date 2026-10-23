namespace GridSystem.Runtime.Interaction.Drop
{
    using Core.Items;
    using Inventories;
    using MessagePipe;
    using Merge;
    using Messaging;
    using UnityEngine;

    /// <summary>
    /// Replaces dragged and target items with the merge result. The result is auto placed into the first
    /// free position of the target inventory, counting the cells of both ingredients as free.
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
        /// Checks that both ingredients are still placed, the recipe exists, the surface can display the result
        /// and the result fits somewhere once the ingredients free their cells. Without the display check
        /// the ingredients would be consumed before the view of the result is requested.
        /// </summary>
        public bool CanHandle(in DropContext context)
        {
            GridItem target = context.TargetItem;
            bool result = target != null
                && target != context.Item
                && context.Item.InventoryId == context.Source.Id
                && target.InventoryId == context.Target.Id
                && _rules.TryGetResult(context.Item.Definition, target.Definition, out IGridItemDefinition definition)
                && context.Surface.CanDisplay(definition)
                && context.Target.TryFindPlacement(definition, context.Item.Id, target.Id, out _, out _);
            return result;
        }

        /// <summary>
        /// Removes ingredients and auto places the result. The position is planned before the removal,
        /// so the cells the ingredients are about to free are already counted as available.
        /// </summary>
        public void Handle(in DropContext context)
        {
            GridItem target = context.TargetItem;
            _rules.TryGetResult(context.Item.Definition, target.Definition, out IGridItemDefinition definition);
            context.Target.TryFindPlacement(definition, context.Item.Id, target.Id, out Vector2Int origin, out int rotation);
            GridItem result = _factory.Create(definition);
            context.Source.TryRemove(context.Item);
            context.Target.TryRemove(target);
            context.Target.TryAdd(result, origin, rotation);
            _publisher.Publish(new ItemsMergedMessage(context.Target, context.Item, target, result));
        }
    }
}
