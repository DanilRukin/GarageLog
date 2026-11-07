namespace GarageLog.Domain.Planning.Purchase;

/// <summary>
/// Статус плана покупки в жизненном цикле.
/// </summary>
public enum PlannedPurchaseStatus
{
    /// <summary>Запланировано: намерение активно.</summary>
    Planned = 1,

    /// <summary>Выполнено: покупка совершена, есть ссылка на <c>Purchase</c>.</summary>
    Done = 2,

    /// <summary>Пропущено: пользователь решил не покупать (без оформления как отмены).</summary>
    Skipped = 3,

    /// <summary>Отменено: план снят с рассмотрения.</summary>
    Cancelled = 4
}
