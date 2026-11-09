using GarageLog.Domain.Catalogs.Parts.Events;
using GarageLog.Domain.Infrastructure;
using GarageLog.SharedKernel;
using GarageLog.SharedKernel.Interfaces;
using static GarageLog.Domain.Errors.PartErrors;

namespace GarageLog.Domain.Catalogs.Parts;

/// <summary>
/// Запчасть или расходник (справочник номенклатуры).
/// Используется для привязки позиций покупок и использованных запчастей к единой карточке.
/// </summary>
public class Part : EntityBase<Guid>, IAgregateRoot
{
    /// <summary>Наименование.</summary>
    public string Name { get; private set; } = default!;

    /// <summary>OEM-артикул (нормализуется: uppercase, без пробелов). Опционально.</summary>
    public string? OemNumber { get; private set; }

    /// <summary>Бренд/производитель. Опционально.</summary>
    public string? Brand { get; private set; }

    /// <summary>Категория.</summary>
    public PartCategory Category { get; private set; }

    /// <summary>Признак активности.</summary>
    public bool IsActive { get; private set; }

    /// <summary>Момент создания (UTC).</summary>
    public DateTimeOffset CreatedAt { get; private init; }

    /// <summary>Момент последнего изменения (UTC).</summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>Конструктор для EF Core.</summary>
    private Part() { }

    /// <summary>
    /// Создаёт новую запчасть.
    /// </summary>
    /// <param name="name">Наименование (непустое, ≤ 256 символов).</param>
    /// <param name="category">Категория.</param>
    /// <param name="oemNumber">OEM-артикул (опционально, ≤ 64 символов).</param>
    /// <param name="brand">Бренд (опционально, ≤ 128 символов).</param>
    public static Part Create(
        string name,
        PartCategory category,
        string? oemNumber = null,
        string? brand = null)
    {
        Validate(name, oemNumber, brand);

        var now = DateTimeOffset.UtcNow;
        var part = new Part
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            OemNumber = NormalizeOem(oemNumber),
            Brand = brand?.Trim(),
            Category = category,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        part.AddDomainEvent(new PartCreated(part.Id, part.Name));
        return part;
    }

    /// <summary>
    /// Обновляет данные запчасти. Доступно только для активных.
    /// </summary>
    /// <param name="name">Новое наименование.</param>
    /// <param name="category">Новая категория.</param>
    /// <param name="oemNumber">Новый OEM-артикул (или null).</param>
    /// <param name="brand">Новый бренд (или null).</param>
    public void Update(string name, PartCategory category, string? oemNumber, string? brand)
    {
        EnsureActive();
        Validate(name, oemNumber, brand);

        Name = name.Trim();
        Category = category;
        OemNumber = NormalizeOem(oemNumber);
        Brand = brand?.Trim();
        Touch();

        AddDomainEvent(new PartUpdated(Id));
    }

    /// <summary>Деактивирует запчасть.</summary>
    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        Touch();
        AddDomainEvent(new PartDeactivated(Id));
    }

    /// <summary>Активирует запчасть.</summary>
    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        Touch();
        AddDomainEvent(new PartActivated(Id));
    }

    /// <summary>Проверяет, что запчасть активна.</summary>
    private void EnsureActive()
    {
        if (!IsActive)
            throw new DomainException<Part>(NotActive);
    }

    /// <summary>Валидирует поля.</summary>
    private static void Validate(string name, string? oemNumber, string? brand)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException<Part>(NameIsEmpty);
        if (name.Trim().Length > 256)
            throw new DomainException<Part>(NameTooLong);
        if (oemNumber is { Length: > 64 })
            throw new DomainException<Part>(OemTooLong);
        if (brand is { Length: > 128 })
            throw new DomainException<Part>(BrandTooLong);
    }

    /// <summary>
    /// Нормализует OEM-артикул: убирает пробелы, приводит к верхнему регистру.
    /// </summary>
    private static string? NormalizeOem(string? oem)
    {
        if (string.IsNullOrWhiteSpace(oem)) return null;
        return oem.Trim().Replace(" ", "").ToUpperInvariant();
    }

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}
