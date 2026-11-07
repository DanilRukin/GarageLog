namespace GarageLog.Domain.Planning.Purchase;

/// <summary>
/// Приоритет плана покупки. Влияет на сортировку в UI и на порог напоминаний.
/// </summary>
public enum PlanningPriority
{
    /// <summary>Низкий: можно отложить.</summary>
    Low = 1,

    /// <summary>Обычный: стандартный уровень.</summary>
    Normal = 2,

    /// <summary>Высокий: желательно купить скоро.</summary>
    High = 3,

    /// <summary>Критический: без этого нельзя эксплуатировать автомобиль.</summary>
    Critical = 4
}
