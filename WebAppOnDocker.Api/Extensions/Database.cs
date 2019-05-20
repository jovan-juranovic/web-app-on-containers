using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Data.SqlClient;
using System.Reflection;
using WebAppOnDocker.Infrastructure;
using WebAppOnDocker.Shared.EventBus.IntegrationEventLogEF;

namespace WebAppOnDocker.Api.Extensions
{
    public static class Database
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseSqlServer(configuration["Database:ConnectionString"],
                                     sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(ApplicationContext).GetTypeInfo().Assembly.GetName().Name);
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });
            });

            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(configuration["Database:ConnectionString"],
                                     sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(ApplicationContext).GetTypeInfo().Assembly.GetName().Name);
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });
            });

            return services;
        }

        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<TContext>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

                try
                {
                    var policy = CreatePolicy(logger);
                    policy.Execute(() => Migrate(seeder, context, services));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database associated with context {DbContextName}", typeof(TContext).Name);
                }

                logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
            }

            return webHost;
        }

        private static Policy CreatePolicy(ILogger logger, int retries = 3)
        {
            return Policy.Handle<SqlException>()
                         .WaitAndRetry(retryCount: retries,
                                       sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                                       onRetry: (ex, timeSpan, retry, ctx) =>
                                       {
                                           logger.LogWarning(ex, "SqlException with message {message} detected on attempt {retry} of {retries}", ex.Message, retry, retries);
                                       });
        }

        private static void Migrate<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services) where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}