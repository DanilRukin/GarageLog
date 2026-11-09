using GarageLog.Domain.Attachments;
using GarageLog.Domain.Catalogs.Parts;
using GarageLog.Domain.Catalogs.TaskTypes;
using GarageLog.Domain.Catalogs.Workshops;
using GarageLog.Domain.Maintenance;
using GarageLog.Domain.Planning.Maintenance;
using GarageLog.Domain.Planning.Purchase;
using GarageLog.Domain.Purchases;
using GarageLog.Domain.Vehicles;
using GarageLog.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace GarageLog.Infrastructure.Persistence;

/// <summary>
/// Основной контекст EF Core для приложения GarageLog.
/// Содержит DbSet-ы для всех агрегатов и применяет их конфигурации.
/// </summary>
public class GarageLogDbContext : DbContext
{
    /// <summary>Создаёт контекст с указанными опциями.</summary>
    public GarageLogDbContext(DbContextOptions<GarageLogDbContext> options)
        : base(options) { }

    /// <summary>Автомобили.</summary>
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    /// <summary>Записи о выполненных/запланированных работах.</summary>
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();

    /// <summary>Покупки.</summary>
    public DbSet<Purchase> Purchases => Set<Purchase>();

    /// <summary>Планы покупок.</summary>
    public DbSet<PlannedPurchase> PlannedPurchases => Set<PlannedPurchase>();

    /// <summary>Плановые работы.</summary>
    public DbSet<PlannedMaintenance> PlannedMaintenances => Set<PlannedMaintenance>();

    /// <summary>Справочник типов работ.</summary>
    public DbSet<TaskType> TaskTypes => Set<TaskType>();

    /// <summary>Справочник СТО.</summary>
    public DbSet<Workshop> Workshops => Set<Workshop>();

    /// <summary>Справочник запчастей.</summary>
    public DbSet<Part> Parts => Set<Part>();

    /// <summary>Вложения (файлы).</summary>
    public DbSet<Attachment> Attachments => Set<Attachment>();

    /// <summary>Исходящие доменные события.</summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Применяет все IEntityTypeConfiguration из сборки Infrastructure
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GarageLogDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
