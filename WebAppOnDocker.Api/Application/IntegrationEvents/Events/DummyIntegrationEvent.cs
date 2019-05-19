using WebAppOnDocker.Shared.EventBus.Events;

namespace WebAppOnDocker.Api.Application.IntegrationEvents.Events
{
    public class DummyIntegrationEvent : IntegrationEvent
    {
        public DummyIntegrationEvent(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }
}