using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using WebAppOnDocker.Shared.EventBus.Abstractions;
using WebAppOnDocker.Shared.EventBus.Events;
using WebAppOnDocker.Shared.EventBus.IntegrationEventLogEF.Utils;

namespace WebAppOnDocker.Shared.EventBus.IntegrationEventLogEF.Services
{
    public class IntegrationEventService : IIntegrationEventService
    {
        private readonly string _appName;
        private readonly IEventBus _bus;
        private readonly DbContext _context;
        private readonly ILogger<IntegrationEventService> _logger;
        private readonly IIntegrationEventLogRepository _integrationEventLogRepository;

        public IntegrationEventService(string appName, 
                                       IEventBus bus,
                                       DbContext context,
                                       ILogger<IntegrationEventService> logger,
                                       Func<DbConnection, IIntegrationEventLogRepository> integrationEventLogRepositoryFactory)
        {
            if (integrationEventLogRepositoryFactory == null)
            {
                throw new ArgumentNullException(nameof(integrationEventLogRepositoryFactory));
            }

            _appName = appName;
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _integrationEventLogRepository = integrationEventLogRepositoryFactory(context.Database.GetDbConnection());
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent @event)
        {
            _logger.LogInformation("Publishing integration event: {integrationEventId} from {appName} - ({@IntegrationEvent})", @event.Id, _appName, @event);

            try
            {
                await _integrationEventLogRepository.MarkEventAsInProgressAsync(@event.Id);

                await _bus.PublishAsync(@event);

                await _integrationEventLogRepository.MarkEventAsPublishedAsync(@event.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {appName} - ({@IntegrationEvent})", @event.Id, _appName, @event);
                await _integrationEventLogRepository.MarkEventAsFailedAsync(@event.Id);
            }
        }

        public async Task SaveChangesIncludingEventLogAsync(IntegrationEvent @event)
        {
            _logger.LogInformation("Saving changes and integrationEvent: {IntegrationEventId} from {appName}", @event.Id, _appName);

            await ResilientTransaction.New(_context).ExecuteAsync(async () =>
            {
                await _context.SaveChangesAsync();
                await _integrationEventLogRepository.SaveEventAsync(@event, _context.Database.CurrentTransaction);
            });
        }
    }
}