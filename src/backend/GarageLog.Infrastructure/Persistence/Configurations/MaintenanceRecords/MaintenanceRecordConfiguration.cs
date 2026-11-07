using GarageLog.Domain.Common;
using GarageLog.Domain.Infrastructure;
using GarageLog.Domain.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarageLog.Infrastructure.Persistence.Configurations.MaintenanceRecords;

public class MaintenanceRecordConfiguration : IEntityTypeConfiguration<MaintenanceRecord>
{
    public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
    {
        builder.ToTable("maintenance_records");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.VehicleId).IsRequired();

        builder.OwnsOne(x => x.TaskType, tt =>
        {
            tt.Property(p => p.TaskTypeId).HasColumnName("task_type_id").IsRequired();
            tt.Property(p => p.NameSnapshot).HasColumnName("task_type_name").HasMaxLength(128).IsRequired();
        });

        builder.Property(x => x.PerformedAt).IsRequired();

        builder.OwnsOne(x => x.OdometerAtService, odo =>
        {
            odo.Property(p => p.Kilometers).HasColumnName("odometer_km").IsRequired();
            odo.Property(p => p.TakenAt).HasColumnName("odometer_taken_at").IsRequired();
            odo.Property(p => p.Source).HasColumnName("odometer_source").HasConversion<int>().IsRequired();
        });

        builder.OwnsOne(x => x.Workshop, w =>
        {
            w.Property(ww => ww.WorkshopId).HasColumnName("workshop_id");
            w.Property(ww => ww.NameSnapshot).HasColumnName("workshop_name").HasMaxLength(128);
        });

        builder.OwnsOne(x => x.LaborCost, m =>
        {
            m.Property(p => p.Amount).HasColumnName("labor_cost_amount").HasPrecision(18, 2).IsRequired();
            m.Property(p => p.Currency).HasColumnName("labor_cost_currency").HasMaxLength(3).IsRequired();
        });

        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        // PartsUsed — owned collection
        builder.OwnsMany(x => x.PartsUsed, p =>
        {
            p.ToTable("record_parts_used");
            p.WithOwner().HasForeignKey("record_id");
            p.Property<int>("id").ValueGeneratedOnAdd();
            p.HasKey("id");

            p.Property(x => x.PartId).HasColumnName("part_id");
            p.Property(x => x.NameSnapshot).HasColumnName("name").HasMaxLength(256).IsRequired();
            p.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 3).IsRequired();
            p.Property(x => x.Unit).HasColumnName("unit").HasConversion<int>().IsRequired();
        });

        // Attachments — owned collection
        builder.OwnsMany(x => x.Attachments, a =>
        {
            a.ToTable("record_attachments");
            a.WithOwner().HasForeignKey("record_id");
            a.Property<int>("id").ValueGeneratedOnAdd();
            a.HasKey("id");

            a.Property(x => x.AttachmentId).HasColumnName("attachment_id").IsRequired();
            a.Property(x => x.Caption).HasColumnName("caption").HasMaxLength(500);
            a.HasIndex("record_id", nameof(AttachmentRef.AttachmentId)).IsUnique();
        });

        // PurchaseIds — primitive collection (EF Core 8+)
        builder.OwnsMany<MaintenancePurchaseLink>("_purchaseLinks", link =>
        {
            link.ToTable("record_purchases");

            link.WithOwner().HasForeignKey("record_id");
            link.HasKey("record_id", nameof(MaintenancePurchaseLink.PurchaseId));

            link.Property(l => l.PurchaseId)
                .HasColumnName("purchase_id")
                .IsRequired();

            link.Property(l => l.LinkedAt)
                .HasColumnName("linked_at")
                .IsRequired();

            link.HasIndex(l => l.PurchaseId);
        });

        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.PurchaseIds);

        builder.HasIndex(x => x.VehicleId);
        builder.HasIndex(x => new { x.VehicleId, x.PerformedAt });
    }
}
