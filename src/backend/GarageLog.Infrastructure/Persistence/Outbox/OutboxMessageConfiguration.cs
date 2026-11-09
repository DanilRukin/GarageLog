using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarageLog.Infrastructure.Persistence.Outbox;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> b)
    {
        b.ToTable("outbox_messages");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Type).HasMaxLength(256).IsRequired();
        b.Property(x => x.Payload).IsRequired();
        b.Property(x => x.OccurredAt).IsRequired();
        b.Property(x => x.ProcessedAt);
        b.Property(x => x.Attempts).IsRequired();
        b.Property(x => x.LastError);

        // Индекс для фонового воркера: неотправленные, по порядку возникновения
        b.HasIndex(x => new { x.ProcessedAt, x.OccurredAt })
            .HasFilter("processed_at IS NULL")
            .HasDatabaseName("ix_outbox_pending");
    }
}
