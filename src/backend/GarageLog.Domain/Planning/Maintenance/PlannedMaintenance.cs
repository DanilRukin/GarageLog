using GarageLog.Domain.Common;
using GarageLog.Domain.Infrastructure;
using GarageLog.Domain.Planning.Maintenance.Events;
using GarageLog.Domain.Planning.Purchase;
using GarageLog.Domain.Planning.Triggers;
using GarageLog.SharedKernel;
using GarageLog.SharedKernel.Interfaces;
using static GarageLog.Domain.Errors.PlannedMaintenanceErrors;

namespace GarageLog.Domain.Planning.Maintenance;

/// <summary>
/// Плановая работа (регламентное ТО, замена расходников и т.п.).
/// Хранит правило срабатывания (<see cref="Trigger"/>), состояние выполнения
/// и напоминания. При выполнении связывается с фактом <c>MaintenanceRecord</c> по Id.
/// </summary>
public class PlannedMaintenance : EntityBase<Guid>, IAgregateRoot
{
    /// <summary>Напоминания, привязанные к плану (owned entity).</summary>
    private readonly List<Reminder> _reminders = new();

    /// <summary>Идентификатор автомобиля, к которому относится план.</summary>
    public Guid VehicleId { get; private init; }

    /// <summary>Тип работы (снапшот имени — на момент создания).</summary>
    public TaskTypeRef TaskType { get; private set; } = default!;

    /// <summary>Правило срабатывания: по дате, по пробегу, по интервалу или комбинированное.</summary>
    public Trigger Trigger { get; private set; } = default!;

    /// <summary>Дата последнего выполнения (для интервальных триггеров). Null — если ещё не выполнялось.</summary>
    public DateTimeOffset? LastPerformedAt { get; private set; }

    /// <summary>Пробег на момент последнего выполнения (для интервальных триггеров). Null — если ещё не выполнялось.</summary>
    public int? LastOdometer { get; private set; }

    /// <summary>Приоритет плана.</summary>
    public PlanningPriority Priority { get; private set; }

    /// <summary>Оценочная стоимость работы. Может быть null.</summary>
    public Money? EstimatedCost { get; private set; }

    /// <summary>Текущий статус.</summary>
    public PlannedMaintenanceStatus Status { get; private set; }

    /// <summary>Дата, до которой план отложен (для статуса Snoozed).</summary>
    public DateTimeOffset? SnoozedUntil { get; private set; }

    /// <summary>Ссылка на созданную запись о работе после выполнения. Null до выполнения.</summary>
    public Guid? LinkedRecordId { get; private set; }

    /// <summary>Свободный комментарий к плану.</summary>
    public string? Notes { get; private set; }

    /// <summary>Момент создания агрегата (UTC).</summary>
    public DateTimeOffset CreatedAt { get; private init; }

    /// <summary>Момент последнего изменения агрегата (UTC).</summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>Напоминания плана (только для чтения).</summary>
    public IReadOnlyCollection<Reminder> Reminders => _reminders.AsReadOnly();

    /// <summary>Конструктор для EF Core.</summary>
    private PlannedMaintenance() { }

    /// <summary>
    /// Создаёт новый план работы.
    /// </summary>
    /// <param name="vehicleId">Идентификатор автомобиля.</param>
    /// <param name="taskType">Тип работы (обязателен).</param>
    /// <param name="trigger">Правило срабатывания (обязателен).</param>
    /// <param name="priority">Приоритет; по умолчанию <see cref="PlanningPriority.Normal"/>.</param>
    /// <param name="estimatedCost">Оценочная стоимость (опционально).</param>
    /// <param name="lastPerformedAt">Дата последнего выполнения (для интервальных триггеров, опционально).</param>
    /// <param name="lastOdometer">Пробег последнего выполнения (для интервальных триггеров, опционально).</param>
    /// <param name="notes">Комментарий (опционально).</param>
    /// <returns>Новый агрегат <see cref="PlannedMaintenance"/>.</returns>
    public static PlannedMaintenance Create(
        Guid vehicleId,
        TaskTypeRef taskType,
        Trigger trigger,
        PlanningPriority priority = PlanningPriority.Normal,
        Money? estimatedCost = null,
        DateTimeOffset? lastPerformedAt = null,
        int? lastOdometer = null,
        string? notes = null)
    {
        if (vehicleId == default(Guid))
            throw new DomainException<PlannedMaintenance>(VehicleIdIsRequired);

        if (taskType is null)
            throw new DomainException<PlannedMaintenance>(TaskTypeRequired);

        EnsureTriggerRequired(trigger);

        ValidateIntervalTriggerAnchor(trigger, lastPerformedAt, lastOdometer);

        if (lastOdometer is < 0)
            throw new DomainException<PlannedMaintenance>(InvalidOdometer);

        var now = DateTimeOffset.UtcNow;
        var plan = new PlannedMaintenance
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicleId,
            TaskType = taskType,
            Trigger = trigger,
            Priority = priority,
            EstimatedCost = estimatedCost,
            LastPerformedAt = lastPerformedAt,
            LastOdometer = lastOdometer,
            Status = PlannedMaintenanceStatus.Planned,
            Notes = notes?.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        plan.AddDomainEvent(new PlannedMaintenanceCreated(
            plan.Id, vehicleId, taskType.TaskTypeId));

        return plan;
    }

    /// <summary>
    /// Обновляет данные плана. Доступно в статусах Planned и Snoozed.
    /// </summary>
    /// <param name="trigger">Новое правило срабатывания.</param>
    /// <param name="priority">Новый приоритет.</param>
    /// <param name="estimatedCost">Новая оценочная стоимость (или null).</param>
    /// <param name="notes">Комментарий.</param>
    public void UpdateDetails(
        Trigger trigger,
        PlanningPriority priority,
        Money? estimatedCost,
        string? notes)
    {
        EnsureEditable();

        EnsureTriggerRequired(trigger);

        ValidateIntervalTriggerAnchor(trigger, LastPerformedAt, LastOdometer);

        Trigger = trigger;
        Priority = priority;
        EstimatedCost = estimatedCost;
        Notes = notes?.Trim();
        Touch();
    }

    /// <summary>
    /// Изменяет только приоритет. Доступно в статусах Planned и Snoozed.
    /// </summary>
    /// <param name="priority">Новый приоритет.</param>
    public void ChangePriority(PlanningPriority priority)
    {
        EnsureEditable();
        Priority = priority;
        Touch();
    }

    /// <summary>
    /// Переводит план в статус «в работе».
    /// </summary>
    public void Start()
    {
        if (Status is not (PlannedMaintenanceStatus.Planned or PlannedMaintenanceStatus.Snoozed))
            throw new DomainException<PlannedMaintenance>(StartOnlyFromPlannedOrSnoozedStatus);

        Status = PlannedMaintenanceStatus.InProgress;
        SnoozedUntil = null;
        Touch();

        AddDomainEvent(new PlannedMaintenanceStarted(Id));
    }

    /// <summary>
    /// Откладывает план на указанную дату.
    /// </summary>
    /// <param name="until">Дата, до которой план отложен. Должна быть в будущем.</param>
    public void Snooze(DateTimeOffset until)
    {
        if (Status is not (PlannedMaintenanceStatus.Planned or PlannedMaintenanceStatus.Snoozed))
            throw new DomainException<PlannedMaintenance>(SnoozeOnlyFromPlannedOrSnoozedStatus);

        if (until <= DateTimeOffset.UtcNow)
            throw new DomainException<PlannedMaintenance>(SnoozeInPast);

        Status = PlannedMaintenanceStatus.Snoozed;
        SnoozedUntil = until;
        Touch();

        AddDomainEvent(new PlannedMaintenanceSnoozed(Id, until));
    }

    /// <summary>
    /// Возвращает план из отложенного в обычный.
    /// </summary>
    public void UnSnooze()
    {
        if (Status != PlannedMaintenanceStatus.Snoozed)
            throw new DomainException<PlannedMaintenance>(NotSnoozed);

        Status = PlannedMaintenanceStatus.Planned;
        SnoozedUntil = null;
        Touch();
    }

    /// <summary>
    /// Помечает план как выполненный, связывает с фактом.
    /// </summary>
    /// <param name="recordId">Идентификатор созданной записи о работе.</param>
    /// <param name="performedAt">Дата выполнения.</param>
    /// <param name="odometerKilometers">Пробег на момент выполнения.</param>
    public void Complete(Guid recordId, DateTimeOffset performedAt, int odometerKilometers)
    {
        if (Status is not (PlannedMaintenanceStatus.Planned
                          or PlannedMaintenanceStatus.InProgress
                          or PlannedMaintenanceStatus.Snoozed))
            throw new DomainException<PlannedMaintenance>(CompleteOnlyFromPlannedInprogressOrSnoozedStatus);

        if (recordId == default)
            throw new DomainException<PlannedMaintenance>(MaintenanceRecordIdIsRequired);

        if (odometerKilometers < 0)
            throw new DomainException<PlannedMaintenance>(InvalidOdometer);

        Status = PlannedMaintenanceStatus.Done;
        LinkedRecordId = recordId;
        LastPerformedAt = performedAt;
        LastOdometer = odometerKilometers;
        SnoozedUntil = null;
        Touch();

        AddDomainEvent(new PlannedMaintenanceCompleted(Id, recordId, performedAt, odometerKilometers));
    }

    /// <summary>
    /// Пропускает план: пользователь решил не выполнять эту итерацию.
    /// </summary>
    /// <param name="reason">Причина (опционально).</param>
    public void Skip(string? reason = null)
    {
        EnsurePlannedOrSnoozed();

        Status = PlannedMaintenanceStatus.Skipped;
        SnoozedUntil = null;
        Touch();

        AddDomainEvent(new PlannedMaintenanceSkipped(Id, reason?.Trim()));
    }

    /// <summary>
    /// Отменяет план.
    /// </summary>
    /// <param name="reason">Причина отмены.</param>
    public void Cancel(string reason)
    {
        EnsurePlannedOrSnoozed();
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException<PlannedMaintenance>(CancellationReasonRequired);

        Status = PlannedMaintenanceStatus.Cancelled;
        SnoozedUntil = null;
        Touch();

        AddDomainEvent(new PlannedMaintenanceCancelled(Id, reason.Trim()));
    }

    // ---------- Напоминания ----------

    /// <summary>
    /// Добавляет напоминание к плану.
    /// </summary>
    /// <param name="reminder">Напоминание.</param>
    public void AddReminder(Reminder reminder)
    {
        EnsureEditable();
        if (reminder is null)
            throw new DomainException<PlannedMaintenance>(ReminderRequired);

        if (_reminders.Any(r => r.Id == reminder.Id))
            return; // идемпотентно

        _reminders.Add(reminder);
        Touch();

        AddDomainEvent(new ReminderAdded(Id, reminder.Id, reminder.RemindAt));
    }

    /// <summary>
    /// Удаляет напоминание по идентификатору.
    /// </summary>
    /// <param name="reminderId">Идентификатор напоминания.</param>
    public void RemoveReminder(Guid reminderId)
    {
        EnsureEditable();
        if (reminderId == default)
            throw new DomainException<PlannedMaintenance>(ReminderIdRequired);

        var existing = _reminders.FirstOrDefault(r => r.Id == reminderId);
        if (existing is null)
            return; // идемпотентно

        _reminders.Remove(existing);
        Touch();
    }

    /// <summary>
    /// Помечает конкретное напоминание отправленным.
    /// </summary>
    /// <param name="reminderId">Идентификатор напоминания.</param>
    /// <param name="sentAt">Момент отправки (UTC).</param>
    public void MarkReminderSent(Guid reminderId, DateTimeOffset sentAt)
    {
        var existing = _reminders.FirstOrDefault(r => r.Id == reminderId)
            ?? throw new DomainException<PlannedMaintenance>(ReminderNotFound);

        existing.MarkSent(sentAt);
        Touch();
    }

    // ---------- Служебное ----------

    /// <summary>
    /// Проверяет, что план редактируем (Planned или Snoozed).
    /// </summary>
    private void EnsureEditable() => EnsurePlannedOrSnoozed();

    /// <summary>Проверяет, что план в статусе Planned или Snoozed.</summary>
    private void EnsurePlannedOrSnoozed()
    {
        if (Status is not (PlannedMaintenanceStatus.Planned or PlannedMaintenanceStatus.Snoozed))
            throw new DomainException<PlannedMaintenance>(OperationAvailableOnlyForPlannedOrSnoozedStatus);
    }

    /// <summary>
    /// Проверяет, что для <see cref="ByIntervalTrigger"/> задан хотя бы один опорный параметр.
    /// </summary>
    private static void ValidateIntervalTriggerAnchor(
        Trigger trigger,
        DateTimeOffset? lastPerformedAt,
        int? lastOdometer)
    {
        if (trigger is not ByIntervalTrigger interval)
            return;

        var needsDate = interval.Months is not null;
        var needsOdo = interval.Km is not null;

        // Для интервального триггера по времени требуется дата последнего выполнения
        if (needsDate && lastPerformedAt is null)
            throw new DomainException<PlannedMaintenance>(IntervalRequiresLastDate);

        // Для интервального триггера по пробегу требуется пробег последнего выполнения
        if (needsOdo && lastOdometer is null)
            throw new DomainException<PlannedMaintenance>(IntervalRequiresLastOdometer);
    }

    private static void EnsureTriggerRequired(Trigger trigger)
    {
        if (trigger is null)
            throw new DomainException<PlannedMaintenance>(TriggerRequired);
    }

    /// <summary>Обновляет <see cref="UpdatedAt"/> текущим временем.</summary>
    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}
