using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectSystem.Domain.Entities;

namespace ProjectSystem.DataAccess.EntityConfigurations
{
    public class LogsConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.ToTable("Logs", tableBuilder => tableBuilder.ExcludeFromMigrations());
        }
    }
}
