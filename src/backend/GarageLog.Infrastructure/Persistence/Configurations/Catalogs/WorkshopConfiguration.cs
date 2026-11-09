using GarageLog.Domain.Catalogs.Workshops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarageLog.Infrastructure.Persistence.Configurations.Catalogs;

/// <summary>EF Core конфигурация агрегата <see cref="Workshop"/>.</summary>
public sealed class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Workshop> b)
    {
        b.ToTable("workshops");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Name).HasMaxLength(256).IsRequired();
        b.Property(x => x.Address).HasMaxLength(512);
        b.Property(x => x.Phone).HasMaxLength(32);
        b.Property(x => x.Website).HasMaxLength(256);
        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt).IsRequired();

        b.Ignore(x => x.DomainEvents);

        b.HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter("is_active = true")
            .HasDatabaseName("ux_workshops_name_active");
    }
}
