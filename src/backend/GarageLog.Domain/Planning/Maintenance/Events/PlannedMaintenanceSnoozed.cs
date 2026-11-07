using GarageLog.SharedKernel;

namespace GarageLog.Domain.Planning.Maintenance.Events;

/// <summary>
/// Плановая работа отложена на новую дату/пробег.
/// </summary>
/// <param name="plannedMaintenanceId">Идентификатор плана.</param>
/// <param name="snoozedUntil">Новая дата отложенного срока (если задана).</param>
public class PlannedMaintenanceSnoozed(
    Guid plannedMaintenanceId,
    DateTimeOffset? snoozedUntil) : DomainEvent
{
    public Guid PlannedMaintenanceId { get; } = plannedMaintenanceId;
    public DateTimeOffset? SnoozedUntil { get; } = snoozedUntil;
}
