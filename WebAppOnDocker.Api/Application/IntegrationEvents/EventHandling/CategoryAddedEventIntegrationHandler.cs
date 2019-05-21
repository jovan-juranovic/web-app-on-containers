using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebAppOnDocker.Api.Application.IntegrationEvents.Events;
using WebAppOnDocker.Core.Model;
using WebAppOnDocker.Infrastructure;
using WebAppOnDocker.Shared.EventBus.Abstractions;

namespace WebAppOnDocker.Api.Application.IntegrationEvents.EventHandling
{
    public class CategoryAddedEventIntegrationHandler : IIntegrationEventHandler<CategoryAddedIntegrationEvent>
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<CategoryAddedEventIntegrationHandler> _logger;

        public CategoryAddedEventIntegrationHandler(ApplicationContext context, ILogger<CategoryAddedEventIntegrationHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Handle(CategoryAddedIntegrationEvent @event)
        {
            _logger.LogInformation("Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            _context.Categories.Add(new Category
            {
                Name = @event.CategoryName,
                Description = "handled integration event",
                CategoryTypeId = CategoryType.AmazonWebServices.Id
            });

            await _context.SaveChangesAsync();
        }
    }
}