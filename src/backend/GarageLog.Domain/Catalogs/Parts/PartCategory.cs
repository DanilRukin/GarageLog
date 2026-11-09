namespace GarageLog.Domain.Catalogs.Parts;

/// <summary>
/// Категория запчасти.
/// </summary>
public enum PartCategory
{
    /// <summary>Масла и технические жидкости.</summary>
    Fluids = 1,

    /// <summary>Фильтры.</summary>
    Filters = 2,

    /// <summary>Двигатель.</summary>
    Engine = 3,

    /// <summary>Подвеска.</summary>
    Suspension = 4,

    /// <summary>Тормоза.</summary>
    Brakes = 5,

    /// <summary>Электрика.</summary>
    Electrical = 6,

    /// <summary>Кузовные детали.</summary>
    Body = 7,

    /// <summary>Шины и диски.</summary>
    Tires = 8,

    /// <summary>Трансмиссия.</summary>
    Transmission = 9,

    /// <summary>Расходники (тряпки, перчатки и т.п.).</summary>
    Consumables = 10,

    /// <summary>Прочее.</summary>
    Other = 99
}
