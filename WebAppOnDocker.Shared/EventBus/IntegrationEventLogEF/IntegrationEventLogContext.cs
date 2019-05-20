using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebAppOnDocker.Shared.EventBus.IntegrationEventLogEF
{
    public class IntegrationEventLogContext : DbContext
    {
        public IntegrationEventLogContext(DbContextOptions<IntegrationEventLogContext> options) : base(options)
        {
        }

        public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IntegrationEventLogEntry>(ConfigureIntegrationEventLogEntry);
        }

        private static void ConfigureIntegrationEventLogEntry(EntityTypeBuilder<IntegrationEventLogEntry> builder)
        {
            builder.ToTable("IntegrationEventLog");

            builder.HasKey(e => e.EventId);

            builder.Property(e => e.EventId)
                   .IsRequired();

            builder.Property(e => e.Content)
                   .IsRequired();

            builder.Property(e => e.CreationTime)
                   .IsRequired();

            builder.Property(e => e.State)
                   .IsRequired();

            builder.Property(e => e.TimesSent)
                   .IsRequired();

            builder.Property(e => e.EventTypeName)
                   .IsRequired();
        }
    }

    public class IntegrationEventLogContextDesignFactory : IDesignTimeDbContextFactory<IntegrationEventLogContext>
    {
        public IntegrationEventLogContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogContext>()
                .UseSqlServer("Server=tcp:web-app-on-containers-sb.database.windows.net,1433;Initial Catalog=web-app-on-containers-db-sb;Persist Security Info=False;User ID=jjuranovic;Password=Enter@01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            return new IntegrationEventLogContext(optionsBuilder.Options);
        }
    }
}