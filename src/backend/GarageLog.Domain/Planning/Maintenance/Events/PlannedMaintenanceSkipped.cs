using GarageLog.SharedKernel;

namespace GarageLog.Domain.Planning.Maintenance.Events;

/// <summary>
/// Плановая работа пропущена (пользователь решил не выполнять).
/// </summary>
/// <param name="plannedMaintenanceId">Идентификатор плана.</param>
/// <param name="reason">Причина (опционально).</param>
public class PlannedMaintenanceSkipped(
    Guid plannedMaintenanceId,
    string? reason) : DomainEvent
{
    public Guid PlannedMaintenanceId { get; } = plannedMaintenanceId;
    public string? Reason { get; } = reason;
}
