using System.Reflection;
using System.Text.Json;

namespace GarageLog.Infrastructure.Persistence.Outbox;

/// <summary>
/// Реализация <see cref="IDomainEventDispatcher"/>: собирает доменные события
/// через рефлексию по свойству <c>DomainEvents</c> и пишет их в outbox.
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        // Полиморфизм для событий, если понадобится, можно включить через TypeInfoResolver
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// <inheritdoc/>
    public int EnqueueDomainEvents(GarageLogDbContext context, CancellationToken cancellationToken = default)
    {
        int count = 0;

        // Перебираем все entry, у которых есть свойство DomainEvents
        foreach (var entry in context.ChangeTracker.Entries())
        {
            var prop = entry
                .GetType()
                .GetProperty("DomainEvents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (prop is null) continue;

            if (prop.GetValue(entry.Entity) is not System.Collections.IEnumerable events) continue;

            bool any = false;
            foreach (var domainEvent in events)
            {
                if (domainEvent is null) continue;
                any = true;
                string? type = domainEvent.GetType().FullName ?? domainEvent.GetType().Name;
                string payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), JsonOptions);
                context.Set<OutboxMessage>().Add(OutboxMessage.Create(type, payload, DateTimeOffset.UtcNow));
                count++;
            }

            // Очищаем события у агрегата, если у него есть метод ClearDomainEvents()
            if (any)
            {
                var clear = entry.Entity.GetType().GetMethod(
                    "ClearDomainEvents",
                    BindingFlags.Public | BindingFlags.Instance);

                clear?.Invoke(entry.Entity, null);
            }
        }

        return count;
    }
}
