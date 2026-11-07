using GarageLog.SharedKernel;

namespace GarageLog.Domain.Planning.Maintenance.Events;

/// <summary>
/// Плановая работа отменена.
/// </summary>
/// <param name="plannedMaintenanceId">Идентификатор плана.</param>
/// <param name="reason">Причина отмены.</param>
public class PlannedMaintenanceCancelled(
    Guid plannedMaintenanceId,
    string reason) : DomainEvent
{
    public Guid PlannedMaintenanceId { get; } = plannedMaintenanceId;
    public string Reason { get; } = reason;
}
