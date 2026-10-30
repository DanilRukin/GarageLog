using GarageLog.Domain.Infrastructure;
using GarageLog.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;
using static GarageLog.Domain.Errors.ReminderErrors;

namespace GarageLog.Domain.Planning;

/// <summary>
/// Нопоминание
/// </summary>
public sealed class Reminder : EntityBase<Guid>
{
    /// <summary>
    /// Дата напоминания
    /// </summary>
    public DateTimeOffset RemindAt { get; private set; }
    /// <summary>
    /// Канал напоминания
    /// </summary>
    public ReminderChannel Channel { get; private set; }
    /// <summary>
    /// Признак отправленности
    /// </summary>
    public bool Sent { get; private set; }
    /// <summary>
    /// Дата, когда отправлено напоминание
    /// </summary>
    public DateTimeOffset SentAt { get; private set; }

    private Reminder() { }

    private Reminder(Guid id, DateTimeOffset remindAt, ReminderChannel channel)
    {
        Id = id;
        RemindAt = remindAt;
        Channel = channel;
        Sent = false;
    }

    public static Reminder Create(DateTimeOffset remindAt, ReminderChannel channel)
    {
        if (remindAt < DateTimeOffset.UtcNow.AddMinutes(-1))
            throw new DomainException<Reminder>(RemindAtDateCouldNotBeInTheFuture);
        return new Reminder(Guid.NewGuid(), remindAt, channel);
    }

    /// <summary>
    /// Помечает напоминание, как отправленное
    /// </summary>
    public void MarkSent(DateTimeOffset sentAt)
    {
        if (Sent) return;
        Sent = true;
        SentAt = sentAt;
    }
}
