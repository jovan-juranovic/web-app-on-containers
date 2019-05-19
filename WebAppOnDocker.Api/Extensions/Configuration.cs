using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebAppOnDocker.Api.Extensions
{
    public static class Configuration
    {
        public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApplicationConfiguration>(configuration);

            return services;
        }
    }
}