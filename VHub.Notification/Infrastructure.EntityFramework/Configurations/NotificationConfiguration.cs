using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        //ef не включает поле "Type" по дэфолту.
        builder.Property("Type").IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
        builder.Property(x => x.Status)
            .HasComment("1 - Pending, 2 - Sent, 3 - Faild")
            .IsRequired();
    }
}
