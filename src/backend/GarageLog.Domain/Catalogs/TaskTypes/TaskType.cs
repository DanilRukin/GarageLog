using GarageLog.Domain.Catalogs.TaskTypes.Events;
using GarageLog.Domain.Infrastructure;
using GarageLog.SharedKernel;
using GarageLog.SharedKernel.Interfaces;
using static GarageLog.Domain.Errors.TaskTypeErrors;

namespace GarageLog.Domain.Catalogs.TaskTypes;

/// <summary>
/// Тип работы (справочник). Определяет категорию, дефолтные интервалы и название,
/// которое используется как снапшот в записях о работах и планах.
/// </summary>
public class TaskType : EntityBase<Guid>, IAgregateRoot
{
    /// <summary>Название типа работы. Уникально среди активных (case-insensitive).</summary>
    public string Name { get; private set; } = default!;

    /// <summary>Категория для группировки.</summary>
    public TaskCategory Category { get; private set; }

    /// <summary>
    /// Дефолтный интервал пробега для этого типа работы (например, 10 000 км для замены масла).
    /// Может быть null, если интервал не применим или задаётся вручную.
    /// </summary>
    public int? DefaultIntervalKm { get; private set; }

    /// <summary>
    /// Дефолтный интервал времени в месяцах (например, 12 месяцев для замены масла).
    /// Может быть null.
    /// </summary>
    public int? DefaultIntervalMonths { get; private set; }

    /// <summary>Признак активности. Деактивированные типы нельзя использовать в новых записях.</summary>
    public bool IsActive { get; private set; }

    /// <summary>Момент создания (UTC).</summary>
    public DateTimeOffset CreatedAt { get; private init; }

    /// <summary>Момент последнего изменения (UTC).</summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>Конструктор для EF Core.</summary>
    private TaskType() { }

    /// <summary>
    /// Создаёт новый тип работы.
    /// </summary>
    /// <param name="name">Название (непустое, ≤ 128 символов).</param>
    /// <param name="category">Категория.</param>
    /// <param name="defaultIntervalKm">Дефолтный интервал пробега (опционально, &gt; 0).</param>
    /// <param name="defaultIntervalMonths">Дефолтный интервал времени в месяцах (опционально, &gt; 0).</param>
    public static TaskType Create(
        string name,
        TaskCategory category,
        int? defaultIntervalKm = null,
        int? defaultIntervalMonths = null)
    {
        ValidateName(name);
        ValidateInterval(defaultIntervalKm, nameof(defaultIntervalKm));
        ValidateInterval(defaultIntervalMonths, nameof(defaultIntervalMonths));

        var now = DateTimeOffset.UtcNow;
        var tt = new TaskType
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Category = category,
            DefaultIntervalKm = defaultIntervalKm,
            DefaultIntervalMonths = defaultIntervalMonths,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        tt.AddDomainEvent(new TaskTypeCreated(tt.Id, tt.Name));
        return tt;
    }

    /// <summary>
    /// Обновляет данные типа работы. Доступно только для активных.
    /// </summary>
    /// <param name="name">Новое название.</param>
    /// <param name="category">Новая категория.</param>
    /// <param name="defaultIntervalKm">Новый интервал пробега (или null).</param>
    /// <param name="defaultIntervalMonths">Новый интервал времени (или null).</param>
    public void Update(
        string name,
        TaskCategory category,
        int? defaultIntervalKm,
        int? defaultIntervalMonths)
    {
        EnsureActive();
        ValidateName(name);
        ValidateInterval(defaultIntervalKm, nameof(defaultIntervalKm));
        ValidateInterval(defaultIntervalMonths, nameof(defaultIntervalMonths));

        Name = name.Trim();
        Category = category;
        DefaultIntervalKm = defaultIntervalKm;
        DefaultIntervalMonths = defaultIntervalMonths;
        Touch();

        AddDomainEvent(new TaskTypeUpdated(Id));
    }

    /// <summary>
    /// Деактивирует тип работы. После деактивации его нельзя использовать в новых записях.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        Touch();
        AddDomainEvent(new TaskTypeDeactivated(Id));
    }

    /// <summary>
    /// Активирует тип работы обратно.
    /// </summary>
    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        Touch();
        AddDomainEvent(new TaskTypeActivated(Id));
    }

    /// <summary>Проверяет, что запись активна.</summary>
    private void EnsureActive()
    {
        if (!IsActive)
            throw new DomainException<TaskType>(NotActive);
    }

    /// <summary>Проверяет название.</summary>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException<TaskType>(NameIsEmpty);
        if (name.Trim().Length > 128)
            throw new DomainException<TaskType>(NameTooLong);
    }

    /// <summary>Проверяет, что интервал положителен, если задан.</summary>
    private static void ValidateInterval(int? value, string paramName)
    {
        if (value is <= 0)
            throw new DomainException<TaskType>(InvalidInterval);
    }

    /// <summary>Обновляет <see cref="UpdatedAt"/>.</summary>
    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}
