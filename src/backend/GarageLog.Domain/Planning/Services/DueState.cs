namespace GarageLog.Domain.Planning.Services;

/// <summary>
/// Статус «насколько срочно» для плана (вычисляемое состояние, не хранится в БД).
/// </summary>
public enum DueState
{
    /// <summary>Не скоро: до срока больше порога.</summary>
    NotDue = 1,

    /// <summary>Скоро: до срока меньше порога, но ещё не наступил.</summary>
    DueSoon = 2,

    /// <summary>Просрочено: срок уже прошёл.</summary>
    Overdue = 3,

    /// <summary>Не применимо: план терминальный или срок не определён.</summary>
    NotApplicable = 4
}
