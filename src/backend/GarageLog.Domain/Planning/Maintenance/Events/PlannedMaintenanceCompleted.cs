using GarageLog.SharedKernel;

namespace GarageLog.Domain.Planning.Maintenance.Events;

/// <summary>
/// Плановая работа выполнена, есть ссылка на факт.
/// </summary>
/// <param name="plannedMaintenanceId">Идентификатор плана.</param>
/// <param name="recordId">Идентификатор созданной записи о работе.</param>
/// <param name="performedAt">Дата выполнения.</param>
/// <param name="odometerKilometers">Пробег на момент выполнения.</param>
public class PlannedMaintenanceCompleted(
    Guid plannedMaintenanceId,
    Guid recordId,
    DateTimeOffset performedAt,
    int odometerKilometers) : DomainEvent
{
    public Guid PlannedMaintenanceId { get; } = plannedMaintenanceId;
    public Guid RecordId { get; } = recordId;
    public DateTimeOffset PerformedAt { get; } = performedAt;
    public int OdometerKilometers { get; } = odometerKilometers;
}
