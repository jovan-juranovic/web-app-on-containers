using System.Threading.Tasks;
using WebAppOnDocker.Api.Application.IntegrationEvents.Events;
using WebAppOnDocker.Shared.EventBus.Abstractions;

namespace WebAppOnDocker.Api.Application.IntegrationEvents.EventHandling
{
    public class DummyIntegrationEventHandler : IIntegrationEventHandler<DummyIntegrationEvent>
    {
        public Task Handle(DummyIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}