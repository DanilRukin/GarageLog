using GarageLog.SharedKernel;

namespace GarageLog.Domain.Planning.Maintenance.Events;

/// <summary>
/// Напоминание добавлено к плановой работе.
/// </summary>
/// <param name="plannedMaintenanceId">Идентификатор плана.</param>
/// <param name="reminderId">Идентификатор напоминания.</param>
/// <param name="remindAt">Момент напоминания.</param>
public class ReminderAdded(
    Guid plannedMaintenanceId,
    Guid reminderId,
    DateTimeOffset remindAt) : DomainEvent
{
    public Guid PlannedMaintenanceId { get; } = plannedMaintenanceId;
    public Guid ReminderId { get; } = reminderId;
    public DateTimeOffset RemindAt { get; } = remindAt;
}
