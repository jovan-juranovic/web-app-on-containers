using WebAppOnDocker.Shared.EventBus.Events;

namespace WebAppOnDocker.Api.Application.IntegrationEvents.Events
{
    public class CategoryAddedIntegrationEvent : IntegrationEvent
    {
        public string CategoryName { get; private set; }

        public CategoryAddedIntegrationEvent(string categoryName)
        {
            CategoryName = categoryName;
        }
    }
}