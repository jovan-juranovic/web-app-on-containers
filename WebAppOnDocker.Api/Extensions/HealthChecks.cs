using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebAppOnDocker.Api.Extensions
{
    public static class HealthChecks
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("liveness", () => HealthCheckResult.Healthy());

            var serviceBusConnectionString = configuration["EventBus:ConnectionString"];
            var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

            hcBuilder.AddSqlServer(configuration["Database:ConnectionString"], name: "web-app-on-containers-db-sb-check", tags: new[] { "web-app-on-containers-db-sb" });
            hcBuilder.AddAzureServiceBusTopic(serviceBusConnection.GetNamespaceConnectionString(), serviceBusConnection.EntityPath, "service-bus-check", tags: new[] {"service-bus"});

            return services;
        }

        public static void UseCustomHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/liveness",
                                new HealthCheckOptions
                                {
                                    Predicate = hc => hc.Name.Contains("liveness")
                                });

            app.UseHealthChecks("/hc",
                                new HealthCheckOptions
                                {
                                    Predicate = _ => true,
                                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                                });
        }
    }
}