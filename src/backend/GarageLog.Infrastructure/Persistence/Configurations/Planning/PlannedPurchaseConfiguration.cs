using GarageLog.Domain.Planning.Purchase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarageLog.Infrastructure.Persistence.Configurations.Planning;

/// <summary>
/// EF Core конфигурация агрегата <see cref="PlannedPurchase"/>.
/// </summary>
public sealed class PlannedPurchaseConfiguration : IEntityTypeConfiguration<PlannedPurchase>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<PlannedPurchase> b)
    {
        b.ToTable("planned_purchases");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever();

        b.Property(x => x.VehicleId);
        b.Property(x => x.PartId);
        b.Property(x => x.Name).HasMaxLength(256).IsRequired();
        b.Property(x => x.Quantity).HasPrecision(18, 3).IsRequired();
        b.Property(x => x.Unit).HasConversion<int>().IsRequired();

        // EstimatedPrice — owned, nullable
        b.OwnsOne(x => x.EstimatedPrice, price =>
        {
            price.Property(p => p.Amount).HasColumnName("estimated_price_amount").HasPrecision(18, 2);
            price.Property(p => p.Currency).HasColumnName("estimated_price_currency").HasMaxLength(3);
        });

        b.Property(x => x.PlannedForDate);
        b.Property(x => x.Priority).HasConversion<int>().IsRequired();
        b.Property(x => x.Status).HasConversion<int>().IsRequired();
        b.Property(x => x.LinkedPurchaseId);
        b.Property(x => x.Notes).HasMaxLength(2000);
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt).IsRequired();

        b.Ignore(x => x.DomainEvents);

        b.HasIndex(x => x.VehicleId);
        b.HasIndex(x => x.Status);
        b.HasIndex(x => new { x.Status, x.PlannedForDate });
        b.HasIndex(x => x.Priority);
    }
}