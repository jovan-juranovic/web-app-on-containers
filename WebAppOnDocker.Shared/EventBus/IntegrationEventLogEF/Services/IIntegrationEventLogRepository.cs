using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAppOnDocker.Shared.EventBus.Events;

namespace WebAppOnDocker.Shared.EventBus.IntegrationEventLogEF.Services
{
    public interface IIntegrationEventLogRepository
    {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId);

        Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);

        Task MarkEventAsPublishedAsync(Guid eventId);

        Task MarkEventAsInProgressAsync(Guid eventId);

        Task MarkEventAsFailedAsync(Guid eventId);
    }
}