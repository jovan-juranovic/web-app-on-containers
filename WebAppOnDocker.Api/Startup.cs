using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using WebAppOnDocker.Api.DependencyInjection;
using WebAppOnDocker.Api.Extensions;

namespace WebAppOnDocker.Api
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation("Application started.");

            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddCorsWithAllowedAnyPolicy();

            services.AddOptions();
            services.AddCustomMvc();
            services.AddCustomHealthChecks(Configuration);
            services.AddApiVersioningWithDefaultOptions();

            services.AddEventBus(Configuration);
            services.AddApplicationConfiguration(Configuration);

            var container = AutofacContainerFactory.Create(services);
            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseExceptionsMiddleware();
            app.UseCorsWithAllowedAnyPolicy();

            app.UseMvc();
            app.UseHttpsRedirection();
            app.UseCustomHealthChecks();

            app.ConfigureEventBus();
        }
    }
}