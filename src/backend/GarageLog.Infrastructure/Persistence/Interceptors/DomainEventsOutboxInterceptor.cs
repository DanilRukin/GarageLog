using GarageLog.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GarageLog.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Интерцептор: перед сохранением собирает доменные события агрегатов
/// и добавляет их в outbox в той же транзакции.
/// </summary>
public class DomainEventsOutboxInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher _dispatcher;

    /// <summary>Создаёт интерцептор.</summary>
    public DomainEventsOutboxInterceptor(IDomainEventDispatcher dispatcher) =>
        _dispatcher = dispatcher;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is GarageLogDbContext ctx)
            _dispatcher.EnqueueDomainEvents(ctx);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is GarageLogDbContext ctx)
            _dispatcher.EnqueueDomainEvents(ctx, cancellationToken);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
