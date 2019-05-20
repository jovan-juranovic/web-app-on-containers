using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAppOnDocker.Core.Model;

namespace WebAppOnDocker.Infrastructure.EntityConfigurations
{
    public class CategoryTypeEntityTypeConfiguration : IEntityTypeConfiguration<CategoryType>
    {
        public void Configure(EntityTypeBuilder<CategoryType> builder)
        {
            builder.ToTable("CategoryType");

            builder.Property(ct => ct.Type)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasData(GetPreconfiguredCategoryTypes());
        }

        private static IEnumerable<CategoryType> GetPreconfiguredCategoryTypes()
        {
            return new List<CategoryType>
            {
                CategoryType.Azure,
                CategoryType.AmazonWebServices,
                CategoryType.GoogleCloudPlatform,
                CategoryType.Heroku
            };
        }
    }
}