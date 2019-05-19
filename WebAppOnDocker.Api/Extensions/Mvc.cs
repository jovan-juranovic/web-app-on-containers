using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using WebAppOnDocker.Api.Utils;

namespace WebAppOnDocker.Api.Extensions
{
    public static class Mvc
    {
        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {
            services.AddRouting(options => options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer));
            services.AddMvc(options => options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())))
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddControllersAsServices();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = CreateInvalidModelStateResponse;
            });

            return services;
        }

        private static IActionResult CreateInvalidModelStateResponse(ActionContext context)
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState);
            var errorMessage = $@"Invalid request. Please refer to the errors for additional details.
Errors: {string.Join(" | ", problemDetails.Errors.Select(kv => $"{string.Join(" | ", kv.Value)}"))}";

            return new BadRequestObjectResult(Envelope.Error(errorMessage)) { ContentTypes = { MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml } };
        }
    }

    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string TransformOutbound(object value)
        {
            return value == null ? null : Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }
}