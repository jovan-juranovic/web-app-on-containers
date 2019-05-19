using System.Threading.Tasks;
using WebAppOnDocker.Shared.EventBus.Events;

namespace WebAppOnDocker.Shared.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}