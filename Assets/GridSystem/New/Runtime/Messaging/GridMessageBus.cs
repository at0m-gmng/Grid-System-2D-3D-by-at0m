namespace GridSystem.Runtime.Messaging
{
    using System;
    using Composition;
    using MessagePipe;

    /// <summary>
    /// Builds isolated MessagePipe brokers and exposes publishers and subscribers to any container.
    /// </summary>
    public static class GridMessageBus
    {
        /// <summary>
        /// Registers publisher and subscriber of every grid message.
        /// </summary>
        public static void Install(IGridRegistrar registrar)
        {
            BuiltinContainerBuilder builder = new BuiltinContainerBuilder();
            builder.AddMessagePipe();
            builder.AddMessageBroker<PointerMovedMessage>();
            builder.AddMessageBroker<PointerButtonMessage>();
            builder.AddMessageBroker<RotateRequestedMessage>();
            builder.AddMessageBroker<ItemAddedMessage>();
            builder.AddMessageBroker<ItemRemovedMessage>();
            builder.AddMessageBroker<ItemMovedMessage>();
            builder.AddMessageBroker<ItemDragStartedMessage>();
            builder.AddMessageBroker<ItemDragUpdatedMessage>();
            builder.AddMessageBroker<ItemDragEndedMessage>();
            builder.AddMessageBroker<ItemHoverMessage>();
            builder.AddMessageBroker<ItemsMergedMessage>();
            builder.AddMessageBroker<SurfaceOpenedMessage>();
            builder.AddMessageBroker<SurfaceClosedMessage>();

            IServiceProvider provider = builder.BuildServiceProvider();
            RegisterBroker<PointerMovedMessage>(registrar, provider);
            RegisterBroker<PointerButtonMessage>(registrar, provider);
            RegisterBroker<RotateRequestedMessage>(registrar, provider);
            RegisterBroker<ItemAddedMessage>(registrar, provider);
            RegisterBroker<ItemRemovedMessage>(registrar, provider);
            RegisterBroker<ItemMovedMessage>(registrar, provider);
            RegisterBroker<ItemDragStartedMessage>(registrar, provider);
            RegisterBroker<ItemDragUpdatedMessage>(registrar, provider);
            RegisterBroker<ItemDragEndedMessage>(registrar, provider);
            RegisterBroker<ItemHoverMessage>(registrar, provider);
            RegisterBroker<ItemsMergedMessage>(registrar, provider);
            RegisterBroker<SurfaceOpenedMessage>(registrar, provider);
            RegisterBroker<SurfaceClosedMessage>(registrar, provider);
        }

        private static void RegisterBroker<TMessage>(IGridRegistrar registrar, IServiceProvider provider)
        {
            registrar.RegisterInstance((IPublisher<TMessage>)provider.GetService(typeof(IPublisher<TMessage>)));
            registrar.RegisterInstance((ISubscriber<TMessage>)provider.GetService(typeof(ISubscriber<TMessage>)));
        }
    }
}
