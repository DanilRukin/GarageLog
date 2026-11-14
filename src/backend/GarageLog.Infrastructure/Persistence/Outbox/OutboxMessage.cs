using System;
using System.Collections.Generic;
using System.Text;

namespace GarageLog.Infrastructure.Persistence.Outbox;

/// <summary>
/// Сообщение исходящего события (transactional outbox).
/// Записывается в той же транзакции, что и изменения агрегатов.
/// Фоновый процесс читает неотправленные сообщения и публикует их во внешнюю шину.
/// </summary>
public class OutboxMessage
{
    /// <summary>Уникальный идентификатор сообщения.</summary>
    public Guid Id { get; private init; }

    /// <summary>Тип события (assembly-qualified name или короткое имя).</summary>
    public string Type { get; private init; } = default!;

    /// <summary>JSON-сериализованное тело события.</summary>
    public string Payload { get; private init; } = default!;

    /// <summary>Момент возникновения события (UTC).</summary>
    public DateTimeOffset OccurredAt { get; private init; }

    /// <summary>Момент успешной публикации. Null — ещё не опубликовано.</summary>
    public DateTimeOffset? ProcessedAt { get; private set; }

    /// <summary>Количество попыток публикации.</summary>
    public int Attempts { get; private set; }

    /// <summary>Последняя ошибка публикации (если была).</summary>
    public string? LastError { get; private set; }

    /// <summary>Конструктор для EF Core.</summary>
    private OutboxMessage() { }

    /// <summary>
    /// Создаёт новое сообщение outbox.
    /// </summary>
    /// <param name="type">Тип события.</param>
    /// <param name="payload">JSON-тело.</param>
    /// <param name="occurredAt">Момент возникновения.</param>
    public static OutboxMessage Create(string type, string payload, DateTimeOffset occurredAt)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = type,
            Payload = payload,
            OccurredAt = occurredAt
        };
    }

    /// <summary>Помечает сообщение как успешно опубликованное.</summary>
    public void MarkProcessed(DateTimeOffset at)
    {
        ProcessedAt = at;
        LastError = null;
        Attempts++;
    }

    /// <summary>Фиксирует неудачную попытку публикации.</summary>
    public void MarkFailed(string error)
    {
        LastError = error;
        Attempts++;
    }
}
