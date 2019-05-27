using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebAppOnDocker.Infrastructure.Http.OfferRequestProcessor
{
    public class OfferRequestProcessorClient : IOfferRequestProcessorClient
    {
        private const string GetEndPoint = "probe";
  
        private readonly HttpClient _httpClient;
        
        public OfferRequestProcessorClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAsync()
        {
            if (_httpClient.BaseAddress == null
                || string.IsNullOrWhiteSpace(_httpClient.BaseAddress.ToString()))
            {
                throw new InvalidOperationException("Base uri is not defined");
            }

            return await _httpClient.GetStringAsync(GetEndPoint);
        }
    }
}