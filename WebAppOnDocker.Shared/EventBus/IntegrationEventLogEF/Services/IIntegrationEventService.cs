using System.Threading.Tasks;
using WebAppOnDocker.Shared.EventBus.Events;

namespace WebAppOnDocker.Shared.EventBus.IntegrationEventLogEF.Services
{
    public interface IIntegrationEventService
    {
        Task PublishThroughEventBusAsync(IntegrationEvent @event);

        Task SaveChangesIncludingEventLogAsync(IntegrationEvent @event);
    }
}