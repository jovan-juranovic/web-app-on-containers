using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAppOnDocker.Shared.EventBus.Events;

namespace WebAppOnDocker.Shared.EventBus.IntegrationEventLogEF.Services
{
    public class IntegrationEventLogRepository : IIntegrationEventLogRepository
    {
        private readonly List<Type> _eventTypes;
        private readonly IntegrationEventLogContext _context;

        /*
         * Reuse existing database connection from main application context
         * instead of creating new context specifically for integration event log
         */
        public IntegrationEventLogRepository(DbConnection dbConnection)
        {
            if(dbConnection == null) throw new ArgumentNullException(nameof(dbConnection));

            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                                  .GetTypes()
                                  .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                                  .ToList();

            _context = new IntegrationEventLogContext(new DbContextOptionsBuilder<IntegrationEventLogContext>().UseSqlServer(dbConnection).Options);
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
        {
            var tid = transactionId.ToString();

            return await _context.IntegrationEventLogs
                                 .Where(entry => entry.TransactionId == tid && entry.State == EventStateEnum.NotPublished)
                                 .OrderBy(entry => entry.CreationTime)
                                 .Select(entry => entry.DeserializeJsonContent(_eventTypes.Find(t => t.Name == entry.EventTypeShortName)))
                                 .ToListAsync();
        }

        public Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), $"{typeof(DbTransaction).FullName} is required as a pre-requisite to save the event.");
            }

            var eventLogEntry = new IntegrationEventLogEntry(@event, transaction.TransactionId);

            _context.Database.UseTransaction(transaction.GetDbTransaction());
            _context.IntegrationEventLogs.Add(eventLogEntry);

            return _context.SaveChangesAsync();
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.Published);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.PublishedFailed);
        }

        private Task UpdateEventStatus(Guid eventId, EventStateEnum status)
        {
            var eventLogEntry = _context.IntegrationEventLogs.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if (status == EventStateEnum.InProgress)
            {
                eventLogEntry.TimesSent++;
            }

            _context.IntegrationEventLogs.Update(eventLogEntry);

            return _context.SaveChangesAsync();
        }
    }
}