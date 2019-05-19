using Autofac;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using WebAppOnDocker.Shared.EventBus.Abstractions;
using WebAppOnDocker.Shared.EventBus.Events;

namespace WebAppOnDocker.Shared.EventBus.ServiceBus
{
    public class EventBusServiceBus : IEventBus
    {
        private const string IntegrationEventSufix = "IntegrationEvent";

        private readonly ILifetimeScope _autofac;
        private readonly ILogger<EventBusServiceBus> _logger;
        private readonly SubscriptionClient _subscriptionClient;
        private readonly IEventBusSubscriptionsManager _subscriptionsManager;
        private readonly IServiceBusPersisterConnection _persisterConnection;

        public EventBusServiceBus(string subscriptionName,
                                  ILifetimeScope autofac,
                                  ILogger<EventBusServiceBus> logger,
                                  IEventBusSubscriptionsManager subscriptionsManager,
                                  IServiceBusPersisterConnection persisterConnection)
        {
            _autofac = autofac;
            _logger = logger;
            _subscriptionsManager = subscriptionsManager;
            _persisterConnection = persisterConnection;

            _subscriptionClient = new SubscriptionClient(persisterConnection.ServiceBusConnectionStringBuilder, subscriptionName);

            RemoveDefaultRule();
            RegisterSubscriptionMessageHandler();
        }

        public void Publish(IntegrationEvent @event)
        {
            var message = CreateMessage(@event);

            var topicClient = _persisterConnection.CreateModel();

            topicClient.SendAsync(message).GetAwaiter().GetResult();
        }

        public async Task PublishAsync(IntegrationEvent @event)
        {
            var message = CreateMessage(@event);

            var topicClient = _persisterConnection.CreateModel();

            await topicClient.SendAsync(message);
        }

        public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name.Replace(IntegrationEventSufix, string.Empty);
            if (_subscriptionsManager.HasSubscriptionsForEvent<T>())
            {
                return;
            }

            try
            {
                _subscriptionClient.AddRuleAsync(new RuleDescription
                                   {
                                       Filter = new CorrelationFilter {Label = eventName},
                                       Name = eventName
                                   })
                                   .GetAwaiter()
                                   .GetResult();
            }
            catch (ServiceBusException)
            {
                _logger.LogWarning("The messaging entity {eventName} already exists.", eventName);
            }

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, nameof(TH));

            _subscriptionsManager.AddSubscription<T, TH>();
        }

        public async Task SubscribeAsync<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name.Replace(IntegrationEventSufix, string.Empty);
            if (_subscriptionsManager.HasSubscriptionsForEvent<T>())
            {
                return;
            }

            try
            {
                await _subscriptionClient.AddRuleAsync(new RuleDescription
                {
                    Filter = new CorrelationFilter {Label = eventName},
                    Name = eventName
                });
            }
            catch (ServiceBusException)
            {
                _logger.LogWarning("The messaging entity {eventName} already exists.", eventName);
            }

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, nameof(TH));

            _subscriptionsManager.AddSubscription<T, TH>();
        }

        public void Unsubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name.Replace(IntegrationEventSufix, string.Empty);

            try
            {
                _subscriptionClient.RemoveRuleAsync(eventName)
                                   .GetAwaiter()
                                   .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogWarning("The messaging entity {eventName} could not be found.", eventName);
            }

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subscriptionsManager.RemoveSubscription<T, TH>();
        }

        public async Task UnsubscribeAsync<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name.Replace(IntegrationEventSufix, string.Empty);

            try
            {
                await _subscriptionClient.RemoveRuleAsync(eventName);
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogWarning("The messaging entity {eventName} could not be found.", eventName);
            }

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subscriptionsManager.RemoveSubscription<T, TH>();
        }

        private static Message CreateMessage(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name.Replace(IntegrationEventSufix, string.Empty);
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            return new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = body,
                Label = eventName
            };
        }

        private void RemoveDefaultRule()
        {
            try
            {
                _subscriptionClient.RemoveRuleAsync(RuleDescription.DefaultRuleName)
                                   .GetAwaiter()
                                   .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogWarning("The messaging entity {DefaultRuleName} Could not be found.", RuleDescription.DefaultRuleName);
            }
        }

        private void RegisterSubscriptionMessageHandler()
        {
            _subscriptionClient.RegisterMessageHandler(async (message, token) =>
                                                       {
                                                           var eventName = $"{message.Label}{IntegrationEventSufix}";
                                                           var messageData = Encoding.UTF8.GetString(message.Body);

                                                           if (await ProcessEvent(eventName, messageData))
                                                           {
                                                               await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                                                           }
                                                       },
                                                       new MessageHandlerOptions(ExceptionReceivedHandler) {MaxConcurrentCalls = 10, AutoComplete = false});
        }

        private async Task<bool> ProcessEvent(string eventName, string message)
        {
            if (!_subscriptionsManager.HasSubscriptionsForEvent(eventName))
            {
                return false;
            }

            using (var scope = _autofac.BeginLifetimeScope())
            {
                var subscriptions = _subscriptionsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {

                    var handler = scope.ResolveOptional(subscription.HandlerType);
                    if (handler == null)
                    {
                        continue;
                    }

                    var eventType = _subscriptionsManager.GetEventTypeByName(eventName);
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    await (Task) concreteType.GetMethod("Handle").Invoke(handler, new[] {integrationEvent});
                }
            }

            return true;
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            _logger.LogError(ex, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message, context);

            return Task.CompletedTask;
        }
    }
}