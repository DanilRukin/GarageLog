namespace GarageLog.Domain.Maintenance;

/// <summary>
/// Статус работы
/// </summary>
public enum MaintenanceStatus
{
    Planned = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4
}
