using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using WebAppOnDocker.Api.Utils;

namespace WebAppOnDocker.Api.Extensions
{
    public static class Versioning
    {
        public static IServiceCollection AddApiVersioningWithDefaultOptions(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ErrorResponses = new ApiVersionExceptionHandler();
            });

            return services;
        }
    }

    public class ApiVersionExceptionHandler : IErrorResponseProvider
    {
        public IActionResult CreateResponse(ErrorResponseContext context)
        {
            return new ObjectResult(Envelope.Error(context.Message))
            {
                StatusCode = context.StatusCode
            };
        }
    }
}