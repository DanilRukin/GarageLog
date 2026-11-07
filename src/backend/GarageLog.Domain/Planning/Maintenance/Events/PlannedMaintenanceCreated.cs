using GarageLog.SharedKernel;

namespace GarageLog.Domain.Planning.Maintenance.Events;

/// <summary>
/// Плановая работа создана.
/// </summary>
/// <param name="plannedMaintenanceId">Идентификатор плана.</param>
/// <param name="vehicleId">Идентификатор автомобиля.</param>
/// <param name="taskTypeId">Идентификатор типа работы.</param>
public class PlannedMaintenanceCreated(
    Guid plannedMaintenanceId,
    Guid vehicleId,
    Guid taskTypeId) : DomainEvent
{
    public Guid PlannedMaintenanceId { get; } = plannedMaintenanceId;
    public Guid VehicleId { get; } = vehicleId;
    public Guid TaskTypeId { get; } = taskTypeId;
}
