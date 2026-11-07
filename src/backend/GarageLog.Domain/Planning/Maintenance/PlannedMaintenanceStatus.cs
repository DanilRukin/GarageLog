namespace GarageLog.Domain.Planning.Maintenance;

/// <summary>
/// Статус плановой работы в жизненном цикле.
/// </summary>
public enum PlannedMaintenanceStatus
{
    /// <summary>Запланировано: правило активно, ожидаем наступления due-момента.</summary>
    Planned = 1,

    /// <summary>В работе: пользователь начал выполнять работу.</summary>
    InProgress = 2,

    /// <summary>Отложено: пользователь сдвинул срок (например, на следующий месяц).</summary>
    Snoozed = 3,

    /// <summary>Выполнено: есть ссылка на <c>MaintenanceRecord</c>.</summary>
    Done = 4,

    /// <summary>Пропущено: пользователь решил не делать эту итерацию.</summary>
    Skipped = 5,

    /// <summary>Отменено: план снят с рассмотрения.</summary>
    Cancelled = 6
}
