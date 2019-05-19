using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebAppOnDocker.Api.Application.IntegrationEvents.Events;
using WebAppOnDocker.Shared.EventBus;
using WebAppOnDocker.Shared.EventBus.Abstractions;
using WebAppOnDocker.Shared.EventBus.ServiceBus;

namespace WebAppOnDocker.Api.Extensions
{
    public static class EventBus
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IServiceBusPersisterConnection>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();

                var serviceBusConnectionString = configuration["EventBus:ConnectionString"];
                var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

                return new DefaultServiceBusPersisterConnection(logger, serviceBusConnection);
            });

            var subscriptionName = configuration["EventBus:SubscriptionName"];
            services.AddSingleton<IEventBus, EventBusServiceBus>(provider =>
            {
                var scope = provider.GetRequiredService<ILifetimeScope>();
                var logger = provider.GetRequiredService<ILogger<EventBusServiceBus>>();
                var persisterConnection = provider.GetRequiredService<IServiceBusPersisterConnection>();
                var subscriptionsManager = provider.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusServiceBus(subscriptionName, scope, logger, subscriptionsManager, persisterConnection);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }

        public static void ConfigureEventBus(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<DummyIntegrationEvent, IIntegrationEventHandler<DummyIntegrationEvent>>();
        }
    }
}