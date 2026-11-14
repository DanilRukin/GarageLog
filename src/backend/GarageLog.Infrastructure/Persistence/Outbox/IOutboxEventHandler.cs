namespace GarageLog.Infrastructure.Persistence.Outbox;

/// <summary>
/// Обработчик outbox-сообщений (in-process). Заменяется на реальную шину по мере роста.
/// </summary>
public interface IOutboxEventHandler
{
    /// <summary>Обрабатывает сообщение.</summary>
    Task HandleAsync(string type, string payload, CancellationToken ct);
}
