namespace GarageLog.Domain.Catalogs.TaskTypes;

/// <summary>
/// Категория типа работы. Используется для группировки в UI и отчётах.
/// </summary>
public enum TaskCategory
{
    /// <summary>Регламентное ТО (масло, фильтры, свечи).</summary>
    ScheduledMaintenance = 1,

    /// <summary>Двигатель и топливная система.</summary>
    Engine = 2,

    /// <summary>Подвеска и рулевое управление.</summary>
    Suspension = 3,

    /// <summary>Тормозная система.</summary>
    Brakes = 4,

    /// <summary>Электрика и электроника.</summary>
    Electrical = 5,

    /// <summary>Кузов и салон.</summary>
    Body = 6,

    /// <summary>Шины и диски.</summary>
    Tires = 7,

    /// <summary>Трансмиссия.</summary>
    Transmission = 8,

    /// <summary>Прочее.</summary>
    Other = 99
}
