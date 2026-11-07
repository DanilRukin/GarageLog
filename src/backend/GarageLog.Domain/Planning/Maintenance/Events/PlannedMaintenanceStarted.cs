using GarageLog.SharedKernel;

namespace GarageLog.Domain.Planning.Maintenance.Events;

/// <summary>
/// Плановая работа перешла в статус «в работе».
/// </summary>
/// <param name="plannedMaintenanceId">Идентификатор плана.</param>
public class PlannedMaintenanceStarted(
    Guid plannedMaintenanceId) : DomainEvent
{
    public Guid PlannedMaintenanceId { get; } = plannedMaintenanceId;
}
