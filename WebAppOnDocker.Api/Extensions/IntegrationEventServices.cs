using System;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebAppOnDocker.Infrastructure;
using WebAppOnDocker.Shared.EventBus.Abstractions;
using WebAppOnDocker.Shared.EventBus.IntegrationEventLogEF.Services;

namespace WebAppOnDocker.Api.Extensions
{
    public static class IntegrationEventServices
    {
        public static IServiceCollection AddIntegrationEventServices(this IServiceCollection services)
        {
            services.AddTransient<IIntegrationEventLogRepository, IntegrationEventLogRepository>();
            services.AddTransient<Func<DbConnection, IIntegrationEventLogRepository>>(provider => conn => new IntegrationEventLogRepository(conn));
            services.AddTransient<IIntegrationEventService, IntegrationEventService>(provider =>
            {
                var bus = provider.GetRequiredService<IEventBus>();
                var context = provider.GetRequiredService<ApplicationContext>();
                var logger = provider.GetRequiredService<ILogger<IntegrationEventService>>();
                var factory = provider.GetRequiredService<Func<DbConnection, IIntegrationEventLogRepository>>();

                return new IntegrationEventService(Program.AppName, bus, context, logger, factory);
            });

            return services;
        }
    }
}