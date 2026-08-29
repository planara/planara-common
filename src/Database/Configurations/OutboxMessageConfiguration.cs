using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planara.Common.Database.Domain;

namespace Planara.Common.Database.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PayloadJson)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.HasIndex(x => x.ProcessedAt);

        builder.HasIndex(x => x.CreatedAt);

        builder.HasIndex(x => x.LockedUntil);

        builder.HasIndex(x => x.Type);

        builder.HasIndex(x => x.TopicKey);
    }
}