using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAppOnDocker.Core.Model;

namespace WebAppOnDocker.Infrastructure.EntityConfigurations
{
    public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Category");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                   .ForSqlServerUseSequenceHiLo("category_hilo")
                   .IsRequired();

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(c => c.Description)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.HasOne(c => c.CategoryType)
                   .WithMany()
                   .HasForeignKey(c => c.CategoryTypeId);
        }
    }
}