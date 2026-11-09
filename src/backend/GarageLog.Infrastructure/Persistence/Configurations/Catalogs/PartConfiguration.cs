using GarageLog.Domain.Catalogs.Parts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarageLog.Infrastructure.Persistence.Configurations.Catalogs;

/// <summary>EF Core конфигурация агрегата <see cref="Part"/>.</summary>
public sealed class PartConfiguration : IEntityTypeConfiguration<Part>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Part> b)
    {
        b.ToTable("parts");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Name).HasMaxLength(256).IsRequired();
        b.Property(x => x.OemNumber).HasMaxLength(64);
        b.Property(x => x.Brand).HasMaxLength(128);
        b.Property(x => x.Category).HasConversion<int>().IsRequired();
        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt).IsRequired();

        b.Ignore(x => x.DomainEvents);

        b.HasIndex(x => x.OemNumber).HasFilter("oem_number IS NOT NULL");
        b.HasIndex(x => x.Name);
    }
}
