using Microsoft.AspNetCore.Builder;
using WebAppOnDocker.Api.Middlewares;

namespace WebAppOnDocker.Api.Extensions
{
    public static class Exceptions
    {
        public static void UseExceptionsMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionsMiddleware>();
        }
    }
}