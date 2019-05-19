using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using System;
using System.IO;
using WebAppOnDocker.Api.Extensions;
using WebAppOnDocker.Infrastructure;

namespace WebAppOnDocker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var appName = typeof(Program).Namespace;
            var configuration = CreateConfiguration();

            var host = CreateWebHost(configuration, args);
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Web host successfully configured ({appName})", appName);

            try
            {
                logger.LogInformation("Applying migrations ({appName})", appName);

                host.MigrateDbContext<IntegrationEventLogContext>((_, __) => { });

                logger.LogInformation("Starting web host ({appName})...", appName);

                host.Run();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Program terminated unexpectedly ({appName})!", appName);
            }
        }

        public static IWebHost CreateWebHost(IConfiguration configuration, string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .CaptureStartupErrors(false)
                          .UseStartup<Startup>()
                          .UseContentRoot(Directory.GetCurrentDirectory())
                          .UseConfiguration(configuration)
                          .ConfigureLogging((hostingContext, builder) =>
                          {
                              builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                              builder.AddConsole();
                              builder.AddDebug();
                              builder.AddApplicationInsights(hostingContext.Configuration["ApplicationInsights:InstrumentationKey"]);

                              builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information)
                                     .AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Error)
                                     .AddFilter<ApplicationInsightsLoggerProvider>("System", LogLevel.Error);
                          })
                          .Build();
        }

        private static IConfiguration CreateConfiguration()
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddEnvironmentVariables();

            var config = builder.Build();

            if (config.GetValue("UseAzureKeyVault", false))
            {
                builder.AddAzureKeyVault($"https://{config["AzureKeyVault:Name"]}.vault.azure.net/",
                                         config["AzureKeyVault:ClientId"],
                                         config["AzureKeyVault:ClientSecret"]);
            }

            return builder.Build();
        }
    }
}