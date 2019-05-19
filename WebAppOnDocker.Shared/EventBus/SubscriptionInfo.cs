using System;

namespace WebAppOnDocker.Shared.EventBus
{
    public partial class InMemoryEventBusSubscriptionsManager
    {
        public class SubscriptionInfo
        {
            private SubscriptionInfo(Type handlerType)
            {
                HandlerType = handlerType;
            }

            public Type HandlerType { get; }

            public static SubscriptionInfo Create(Type handlerType)
            {
                return new SubscriptionInfo(handlerType);
            }
        }
    }
}