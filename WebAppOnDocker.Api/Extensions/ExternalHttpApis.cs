using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using WebAppOnDocker.Infrastructure.Http.OfferRequestProcessor;

namespace WebAppOnDocker.Api.Extensions
{
    public static class ExternalHttpApis
    {
        private const int RetryCount = 6;
        public static void AddExternalHttpApis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IOfferRequestProcessorClient, OfferRequestProcessorClient>(client =>
                client.BaseAddress = new Uri(configuration["OfferRequestProcessor:BaseUrl"]))
                .AddPolicyHandler(GetRetryPolicy());
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var jitterer = new Random();

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,retryAttempt))
                                                      + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)));
        }
    }
}