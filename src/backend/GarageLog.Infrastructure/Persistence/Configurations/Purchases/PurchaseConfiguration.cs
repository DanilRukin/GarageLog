using GarageLog.Domain.Purchases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarageLog.Infrastructure.Persistence.Configurations.Purchases;

/// <summary>
/// EF Core конфигурация агрегата <see cref="Purchase"/>.
/// </summary>
public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Purchase> b)
    {
        b.ToTable("purchases");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.VehicleId);
        b.Property(x => x.PurchasedAt).IsRequired();
        b.Property(x => x.Vendor).HasMaxLength(256).IsRequired();
        b.Property(x => x.Notes).HasMaxLength(2000);

        // Money — owned
        b.OwnsOne(x => x.TotalAmount, m =>
        {
            m.Property(p => p.Amount).HasColumnName("total_amount").HasPrecision(18, 2).IsRequired();
            m.Property(p => p.Currency).HasColumnName("total_currency").HasMaxLength(3).IsRequired();
        });

        // AttachmentRef — owned, nullable
        b.OwnsOne(x => x.Receipt, r =>
        {
            r.Property(p => p.AttachmentId).HasColumnName("receipt_attachment_id");
            r.Property(p => p.Caption).HasColumnName("receipt_caption").HasMaxLength(500);
        });

        b.Property(x => x.Status).HasConversion<int>().IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt).IsRequired();

        // Позиции — owned collection
        b.OwnsMany(x => x.Lines, line =>
        {
            line.ToTable("purchase_lines");
            line.WithOwner().HasForeignKey("purchase_id");
            line.Property<int>("id").ValueGeneratedOnAdd();
            line.HasKey("id");

            line.Property(l => l.PartId).HasColumnName("part_id");
            line.Property(l => l.NameSnapshot).HasColumnName("name").HasMaxLength(256).IsRequired();
            line.Property(l => l.Quantity).HasColumnName("quantity").HasPrecision(18, 3).IsRequired();
            line.Property(l => l.Unit).HasColumnName("unit").HasConversion<int>().IsRequired();

            line.OwnsOne(l => l.UnitPrice, price =>
            {
                price.Property(p => p.Amount).HasColumnName("unit_price_amount").HasPrecision(18, 2).IsRequired();
                price.Property(p => p.Currency).HasColumnName("unit_price_currency").HasMaxLength(3).IsRequired();
            });

            // LineTotal — вычисляемое, не маппим
            line.Ignore(l => l.LineTotal);

            line.HasIndex("purchase_id");


        });

        b.Ignore(x => x.DomainEvents);

        b.HasIndex(x => x.VehicleId);
        b.HasIndex(x => x.PurchasedAt);
        b.HasIndex(x => x.Status);
    }
}
