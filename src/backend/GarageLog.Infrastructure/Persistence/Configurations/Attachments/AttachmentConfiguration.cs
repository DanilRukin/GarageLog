using GarageLog.Domain.Attachments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarageLog.Infrastructure.Persistence.Configurations.Attachments;

/// <summary>EF Core конфигурация агрегата <see cref="Attachment"/>.</summary>
public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Attachment> b)
    {
        b.ToTable("attachments");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.FileName).HasMaxLength(256).IsRequired();
        b.Property(x => x.MimeType).HasMaxLength(128).IsRequired();
        b.Property(x => x.SizeBytes).IsRequired();
        b.Property(x => x.StorageKey).HasMaxLength(512);

        // ContentHash — VO через value converter
        b.Property(x => x.ContentHash)
            .HasConversion(h => h.Value, s => ContentHash.From(s))
            .HasColumnName("content_hash")
            .HasMaxLength(ContentHash.HexLength)
            .IsRequired();

        b.Property(x => x.Status).HasConversion<int>().IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt).IsRequired();
        b.Property(x => x.ActivatedAt);
        b.Property(x => x.DeletedAt);
        b.Property(x => x.DeletedReason).HasMaxLength(512);

        b.Ignore(x => x.DomainEvents);

        b.HasIndex(x => x.ContentHash)
            .HasDatabaseName("ix_attachments_content_hash");

        b.HasIndex(x => x.Status)
            .HasDatabaseName("ix_attachments_status");

        // Частичный индекс для «висящих» Pending-вложений (для фоновой очистки)
        b.HasIndex(x => x.CreatedAt)
            .HasFilter("status = 1")
            .HasDatabaseName("ix_attachments_pending_created_at");
    }
}
