using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GarageLog.Infrastructure.Persistence.Outbox;

/// <summary>
/// Фоновый процесс, публикующий outbox-сообщения во внешнюю шину.
/// В MVP «публикация» может быть просто in-process event bus / log.
/// </summary>
public class OutboxPublisher : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ILogger<OutboxPublisher> _logger;
    private TimeSpan _interval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 50;

    public OutboxPublisher(IServiceScopeFactory scopes, ILogger<OutboxPublisher> logger)
    {
        _scopes = scopes;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PublishBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка публикации outbox");
            }

            try { await Task.Delay(_interval, stoppingToken); }
            catch (TaskCanceledException) { break; }
        }
    }

    private async Task PublishBatchAsync(CancellationToken ct)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<GarageLogDbContext>();
        var handler = scope.ServiceProvider.GetRequiredService<IOutboxEventHandler>();

        var batch = await db.OutboxMessages
            .Where(o => o.ProcessedAt == null)
            .OrderBy(o => o.OccurredAt)
            .Take(BatchSize)
            .ToListAsync(ct);
        if (batch.Count == 0) return;
        foreach (var msg in batch)
        {
            try
            {
                if (handler is not null)
                    await handler.HandleAsync(msg.Type, msg.Payload, ct);
                msg.MarkProcessed(DateTimeOffset.UtcNow);
            }
            catch (Exception ex)
            {
                msg.MarkFailed(ex.Message);
                _logger.LogWarning(ex, "Не удалось обработать outbox {Id}", msg.Id);
            }
        }
        await db.SaveChangesAsync(ct);
    }
}
