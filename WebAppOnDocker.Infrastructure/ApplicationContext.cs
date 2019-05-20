using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WebAppOnDocker.Core.Model;
using WebAppOnDocker.Infrastructure.EntityConfigurations;

namespace WebAppOnDocker.Infrastructure
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryType> CategoryTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CategoryTypeEntityTypeConfiguration());
            builder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
        }
    }

    public class ApplicationContextDesignFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>()
                .UseSqlServer("Server=tcp:web-app-on-containers-sb.database.windows.net,1433;Initial Catalog=web-app-on-containers-db-sb;Persist Security Info=False;User ID=jjuranovic;Password=Enter@01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}