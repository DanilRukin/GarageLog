namespace GarageLog.Infrastructure.Persistence.Outbox;

/// <summary>
/// Диспетчер доменных событий: сериализует события агрегатов в outbox-сообщения.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Собирает доменные события из всех отслеживаемых агрегатов,
    /// сериализует их и добавляет в outbox (не сохраняя в БД).
    /// </summary>
    /// <param name="context">Контекст EF Core.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Количество добавленных сообщений.</returns>
    int EnqueueDomainEvents(GarageLogDbContext context, CancellationToken cancellationToken = default);
}
