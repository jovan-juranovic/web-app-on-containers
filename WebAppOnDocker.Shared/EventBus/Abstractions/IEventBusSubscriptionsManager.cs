using System;
using System.Collections.Generic;
using WebAppOnDocker.Shared.EventBus.Events;
using static WebAppOnDocker.Shared.EventBus.InMemoryEventBusSubscriptionsManager;

namespace WebAppOnDocker.Shared.EventBus.Abstractions
{
    public interface IEventBusSubscriptionsManager
    {
        void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;

        bool HasSubscriptionsForEvent(string eventName);

        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;

        Type GetEventTypeByName(string eventName);
    }
}