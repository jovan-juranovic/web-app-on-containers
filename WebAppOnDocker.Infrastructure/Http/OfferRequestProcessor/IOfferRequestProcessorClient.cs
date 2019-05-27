using System.Threading.Tasks;

namespace WebAppOnDocker.Infrastructure.Http.OfferRequestProcessor
{
    public interface IOfferRequestProcessorClient
    {
        Task<string> GetAsync();
    }
}