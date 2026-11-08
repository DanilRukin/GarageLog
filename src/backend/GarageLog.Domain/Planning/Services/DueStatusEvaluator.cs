namespace GarageLog.Domain.Planning.Services;

/// <summary>
/// Оценивает срочность плана на основе вычисленного next due и текущего
/// времени/пробега автомобиля.
/// </summary>
public class DueStatusEvaluator
{
    private readonly TimeProvider _time;
    private readonly TimeSpan _soonThreshold;

    /// <summary>
    /// Создаёт оценщик.
    /// </summary>
    /// <param name="time">Источник времени.</param>
    /// <param name="soonThreshold">За сколько до срока считать «скоро»; по умолчанию 14 дней.</param>
    public DueStatusEvaluator(TimeProvider time, TimeSpan? soonThreshold = null)
    {
        _time = time;
        _soonThreshold = soonThreshold ?? TimeSpan.FromDays(14);
    }

    /// <summary>
    /// Оценивает срочность плана.
    /// </summary>
    /// <param name="due">Вычисленный next due.</param>
    /// <param name="currentOdometer">Текущий пробег автомобиля (если известен).</param>
    /// <param name="odometerSoonThreshold">За сколько км до срока считать «скоро»; по умолчанию 1000 км.</param>
    public DueState Evaluate(NextDue due, int? currentOdometer, int odometerSoonThreshold = 1000)
    {
        if (due.IsUndefined) return DueState.NotApplicable;
        var now = _time.GetUtcNow();

        // Дата: overdue, если due-дата в прошлом; dueSoon, если в пределах порога.
        if (due.DueDate is { } d)
        {
            if (d < now) return DueState.Overdue;
            if (d - now < _soonThreshold) return DueState.DueSoon;
        }

        // Пробег: overdue, если текущий пробег >= due-пробег; dueSoon, если близко.
        if (due.DueOdometer is { } o && currentOdometer is { } cur)
        {
            if (cur >= o) return DueState.Overdue;
            if (o - cur <= odometerSoonThreshold) return DueState.DueSoon;
        }

        return DueState.NotDue;
    }
}
