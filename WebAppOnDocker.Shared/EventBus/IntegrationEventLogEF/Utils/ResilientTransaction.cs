using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace WebAppOnDocker.Shared.EventBus.IntegrationEventLogEF.Utils
{
    /*
     * Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction()
     * See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
     */
    public class ResilientTransaction
    {
        private readonly DbContext _context;

        private ResilientTransaction(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static ResilientTransaction New(DbContext context) => new ResilientTransaction(context);

        public async Task ExecuteAsync(Func<Task> action)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    await action();
                    transaction.Commit();
                }
            });
        }
    }
}