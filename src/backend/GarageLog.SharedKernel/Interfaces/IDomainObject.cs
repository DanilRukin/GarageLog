namespace GarageLog.SharedKernel.Interfaces;

/// <summary>
/// Объект домена
/// </summary>
public interface IDomainObject
{
    /// <summary>
    /// Доменные события
    /// </summary>
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }

    /// <summary>
    /// Добавляет доменное событие
    /// </summary>
    /// <param name="domainEvent">Доменное событие</param>
    void AddDomainEvent(DomainEvent domainEvent);

    /// <summary>
    /// Удаляет доменное событие
    /// </summary>
    /// <param name="domainEvent">Доменное событие</param>
    void RemoveDomainEvent(DomainEvent domainEvent);

    /// <summary>
    /// Очищает доменные события
    /// </summary>
    void ClearDomainEvents();
}
