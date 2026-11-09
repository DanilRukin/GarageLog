using GarageLog.Domain.Catalogs.TaskTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarageLog.Infrastructure.Persistence.Configurations.Catalogs;

/// <summary>EF Core конфигурация агрегата <see cref="TaskType"/>.</summary>
public sealed class TaskTypeConfiguration : IEntityTypeConfiguration<TaskType>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<TaskType> b)
    {
        b.ToTable("task_types");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Name).HasMaxLength(128).IsRequired();
        b.Property(x => x.Category).HasConversion<int>().IsRequired();
        b.Property(x => x.DefaultIntervalKm);
        b.Property(x => x.DefaultIntervalMonths);
        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt).IsRequired();

        b.Ignore(x => x.DomainEvents);

        // Уникальность активного имени (case-insensitive) — через функциональный индекс
        // (для Postgres — lower(name) WHERE is_active; для MSSQL — filtered index + collation CI).
        // Простейший переносимый вариант — обычный unique index по (name), но он блокирует
        // повторное использование имени после деактивации. Поэтому — filtered unique index:
        b.HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter("is_active = true")
            .HasDatabaseName("ux_task_types_name_active");
    }
}
