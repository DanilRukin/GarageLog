using GarageLog.Domain.Planning.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GarageLog.Infrastructure.Persistence.Configurations.Planning;

/// <summary>
/// EF Core конфигурация агрегата <see cref="PlannedMaintenance"/>.
/// </summary>
public sealed class PlannedMaintenanceConfiguration : IEntityTypeConfiguration<PlannedMaintenance>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<PlannedMaintenance> b)
    {
        b.ToTable("planned_maintenance");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.VehicleId).IsRequired();

        // TaskType — owned
        b.OwnsOne(x => x.TaskType, tt =>
        {
            tt.Property(p => p.TaskTypeId).HasColumnName("task_type_id").IsRequired();
            tt.Property(p => p.NameSnapshot).HasColumnName("task_type_name").HasMaxLength(128).IsRequired();
        });

        // Trigger — иерархия. Используем TPH через discriminator + nullable-поля.
        ConfigureTrigger(b);

        b.Property(x => x.LastPerformedAt);
        b.Property(x => x.LastOdometer);
        b.Property(x => x.Priority).HasConversion<int>().IsRequired();

        // EstimatedCost — owned, nullable
        b.OwnsOne(x => x.EstimatedCost, cost =>
        {
            cost.Property(p => p.Amount).HasColumnName("estimated_cost_amount").HasPrecision(18, 2);
            cost.Property(p => p.Currency).HasColumnName("estimated_cost_currency").HasMaxLength(3);
        });

        b.Property(x => x.Status).HasConversion<int>().IsRequired();
        b.Property(x => x.SnoozedUntil);
        b.Property(x => x.LinkedRecordId);
        b.Property(x => x.Notes).HasMaxLength(2000);
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt).IsRequired();

        // Reminders — owned collection
        b.OwnsMany(x => x.Reminders, r =>
        {
            r.ToTable("planned_maintenance_reminders");
            r.WithOwner().HasForeignKey("planned_id");
            r.Property<Guid>("id").ValueGeneratedNever();
            r.HasKey("id");

            r.Property(x => x.Id).HasColumnName("reminder_id").IsRequired();
            r.Property(x => x.RemindAt).HasColumnName("remind_at").IsRequired();
            r.Property(x => x.Channel).HasColumnName("channel").HasConversion<int>().IsRequired();
            r.Property(x => x.Sent).HasColumnName("sent").IsRequired();
            r.Property(x => x.SentAt).HasColumnName("sent_at");

            r.HasIndex("planned_id");
        });

        b.Ignore(x => x.DomainEvents);

        b.HasIndex(x => x.VehicleId);
        b.HasIndex(x => x.Status);
        b.HasIndex(x => new { x.Status, x.Priority });
    }

    /// <summary>
    /// Конфигурирует иерархию <see cref="Trigger"/> через TPH.
    /// </summary>
    private static void ConfigureTrigger(EntityTypeBuilder<PlannedMaintenance> b)
    {
        // Trigger — JSON-колонка (value converter, без OwnsOne)
        b.Property(x => x.Trigger)
            .HasConversion(new TriggerJsonConverter(JsonOptions))
            .HasColumnName("trigger")
            .HasColumnType("jsonb")          // PostgreSQL; для MSSQL — "nvarchar(max)"
            .IsRequired();
    }
}
