using GarageLog.Domain.Catalogs.Workshops.Events;
using GarageLog.Domain.Infrastructure;
using GarageLog.SharedKernel;
using GarageLog.SharedKernel.Interfaces;
using static GarageLog.Domain.Errors.WorkshopErrors;

namespace GarageLog.Domain.Catalogs.Workshops;

/// <summary>
/// СТО/сервис (справочник). Используется как снапшот в записях о работах.
/// </summary>
public class Workshop : EntityBase<Guid>, IAgregateRoot
{
    /// <summary>Название СТО. Уникально среди активных.</summary>
    public string Name { get; private set; } = default!;

    /// <summary>Адрес (опционально).</summary>
    public string? Address { get; private set; }

    /// <summary>Контактный телефон (опционально).</summary>
    public string? Phone { get; private set; }

    /// <summary>Сайт (опционально).</summary>
    public string? Website { get; private set; }

    /// <summary>Признак активности.</summary>
    public bool IsActive { get; private set; }

    /// <summary>Момент создания (UTC).</summary>
    public DateTimeOffset CreatedAt { get; private init; }

    /// <summary>Момент последнего изменения (UTC).</summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>Конструктор для EF Core.</summary>
    private Workshop() { }

    /// <summary>
    /// Создаёт новую СТО.
    /// </summary>
    /// <param name="name">Название (непустое, ≤ 256 символов).</param>
    /// <param name="address">Адрес (опционально, ≤ 512 символов).</param>
    /// <param name="phone">Телефон (опционально, ≤ 32 символов).</param>
    /// <param name="website">Сайт (опционально, ≤ 256 символов).</param>
    public static Workshop Create(
        string name,
        string? address = null,
        string? phone = null,
        string? website = null)
    {
        Validate(name, address, phone, website);

        var now = DateTimeOffset.UtcNow;
        var w = new Workshop
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Address = address?.Trim(),
            Phone = phone?.Trim(),
            Website = website?.Trim(),
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        w.AddDomainEvent(new WorkshopCreated(w.Id, w.Name));
        return w;
    }

    /// <summary>
    /// Обновляет данные СТО. Доступно только для активных.
    /// </summary>
    /// <param name="name">Новое название.</param>
    /// <param name="address">Новый адрес.</param>
    /// <param name="phone">Новый телефон.</param>
    /// <param name="website">Новый сайт.</param>
    public void Update(string name, string? address, string? phone, string? website)
    {
        EnsureActive();
        Validate(name, address, phone, website);

        Name = name.Trim();
        Address = address?.Trim();
        Phone = phone?.Trim();
        Website = website?.Trim();
        Touch();

        AddDomainEvent(new WorkshopUpdated(Id));
    }

    /// <summary>Деактивирует СТО.</summary>
    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        Touch();
        AddDomainEvent(new WorkshopDeactivated(Id));
    }

    /// <summary>Активирует СТО.</summary>
    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        Touch();
        AddDomainEvent(new WorkshopActivated(Id));
    }

    /// <summary>Проверяет, что СТО активна.</summary>
    private void EnsureActive()
    {
        if (!IsActive)
            throw new DomainException<Workshop>(NotActive);
    }

    /// <summary>Валидирует все поля.</summary>
    private static void Validate(string name, string? address, string? phone, string? website)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException<Workshop>(NameIsEmpty);
        if (name.Trim().Length > 256)
            throw new DomainException<Workshop>(NameTooLong);
        if (address is { Length: > 512 })
            throw new DomainException<Workshop>(AddressTooLong);
        if (phone is { Length: > 32 })
            throw new DomainException<Workshop>(PhoneTooLong);
        if (website is { Length: > 256 })
            throw new DomainException<Workshop>(WebsiteTooLong);
    }

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}
