using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WebAppOnDocker.Api.Extensions
{
    public static class Cors
    {
        private const string AllowedAnyPolicy = "AllowedAnyPolicy";

        public static IServiceCollection AddCorsWithAllowedAnyPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(AllowedAnyPolicy,
                                  builder => builder.SetIsOriginAllowed(host => true)
                                                    .AllowAnyHeader()
                                                    .AllowAnyMethod()
                                                    .AllowCredentials());
            });

            return services;
        }

        public static void UseCorsWithAllowedAnyPolicy(this IApplicationBuilder app)
        {
            app.UseCors(AllowedAnyPolicy);
        }
    }
}