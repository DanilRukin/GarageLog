namespace GarageLog.SharedKernel.Interfaces;

/// <summary>
/// Диспетчер событий домена
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Публикуте доменные события сущностей домена
    /// </summary>
    /// <param name="entities">Доменные сущности</param>
    Task DispatchAndClearEvents(IEnumerable<IDomainObject> entities);
}