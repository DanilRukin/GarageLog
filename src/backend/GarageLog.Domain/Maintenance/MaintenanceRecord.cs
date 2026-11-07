using GarageLog.Domain.Common;
using GarageLog.Domain.Infrastructure;
using GarageLog.Domain.Maintenance.Events;
using GarageLog.Domain.Vehicles;
using GarageLog.SharedKernel;
using GarageLog.SharedKernel.Interfaces;
using static GarageLog.Domain.Errors.MaintenanceRecordErrors;

namespace GarageLog.Domain.Maintenance;

/// <summary>
/// Факт выполненной (или выполняемой) работы
/// </summary>
public class MaintenanceRecord : EntityBase<Guid>, IAgregateRoot
{
    private readonly List<PartUsage> _partsUsed = new();
    private readonly List<AttachmentRef> _attachments = new();
    private readonly List<MaintenancePurchaseLink> _purchaseLinks = new(); // не менять имя, т.к. используется для конфигурации в EF Core
    /// <summary>
    /// Id авто
    /// </summary>
    public Guid VehicleId { get; private set; }
    /// <summary>
    /// Тип работ
    /// </summary>
    public TaskTypeRef TaskType { get; private set; } = default!;
    /// <summary>
    /// Дата выполнения работ
    /// </summary>
    public DateTimeOffset PerformedAt { get; private set; }
    /// <summary>
    /// Показания пробега на момент выполнения работ
    /// </summary>
    public OdometerReading OdometerAtService { get; private set; } = default!;

    public WorkshopRef? Workshop { get; private set; }
    /// <summary>
    /// Описание
    /// </summary>
    public string? Description { get; private set; }
    /// <summary>
    /// Стоимость работ
    /// </summary>
    public Money LaborCost { get; private set; } = default!;
    /// <summary>
    /// Статус работ
    /// </summary>
    public MaintenanceStatus Status { get; private set; }
    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }
    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyCollection<PartUsage> PartsUsed => _partsUsed.AsReadOnly();
    public IReadOnlyCollection<AttachmentRef> Attachments => _attachments.AsReadOnly();
    public IReadOnlyCollection<Guid> PurchaseIds => _purchaseLinks.Select(l => l.PurchaseId).ToList();

    private MaintenanceRecord() { }

    public static MaintenanceRecord Create(
        Guid vehicleId,
        TaskTypeRef taskType,
        DateTimeOffset performedAt,
        OdometerReading odometerAtService,
        Money laborCost,
        WorkshopRef? workshop = null,
        string? description = null,
        MaintenanceStatus initialStatus = MaintenanceStatus.Planned)
    {
        if (vehicleId == default(Guid))
            throw new DomainException<MaintenanceRecord>(RecordsVehicleIdCanNotBeDefault);
        if (laborCost is null)
            throw new DomainException<MaintenanceRecord>(LaborCostIsNull);
        EnsureDateNotFuture(performedAt);
        if (initialStatus is MaintenanceStatus.Completed or MaintenanceStatus.Cancelled)
            throw new DomainException<MaintenanceRecord>(InvalidInitialStatus);

        DateTimeOffset now = DateTimeOffset.UtcNow;
        MaintenanceRecord record = new MaintenanceRecord
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicleId,
            TaskType = taskType,
            PerformedAt = performedAt,
            OdometerAtService = odometerAtService,
            Workshop = workshop,
            Description = description?.Trim(),
            LaborCost = laborCost,
            Status = initialStatus,
            CreatedAt = now,
            UpdatedAt = now
        };

        record.AddDomainEvent(new MaintenanceRecordCreated(
            record.Id, vehicleId, taskType.TaskTypeId, performedAt, odometerAtService.Kilometers));
        return record;
    }

    /// <summary>
    /// Редактирование
    /// </summary>
    public void UpdateDetails(
        DateTimeOffset performedAt,
        OdometerReading odometerAtService,
        Money laborCost,
        WorkshopRef? workshop,
        string? description
        )
    {
        EnsureEditable();
        EnsureDateNotFuture(performedAt);
        if (laborCost is null)
            throw new DomainException<MaintenanceRecord>(LaborCostIsRequired);
        PerformedAt = performedAt;
        OdometerAtService = odometerAtService;
        LaborCost = laborCost;
        Workshop = workshop;
        Description = description?.Trim();
        Touch();
    }

    /// <summary>
    /// Добавляет запчасть
    /// </summary>
    public void AddPart(PartUsage part)
    {
        EnsureEditable();
        if (part is null)
            throw new DomainException<MaintenanceRecord>(PartIsRequired);
        if (_partsUsed.Any(p =>
                p.PartId == part.PartId &&
                string.Equals(p.NameSnapshot, part.NameSnapshot, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException<MaintenanceRecord>(PartIsAlreadyAdded);
        _partsUsed.Add(part);
        Touch();
    }

    /// <summary>
    /// Удаляет запчасть
    /// </summary>
    public void RemovePart(Guid? partId, string nameSnapshot)
    {
        EnsureEditable();
        var existing = _partsUsed.FirstOrDefault(p =>
            p.PartId == partId &&
            string.Equals(p.NameSnapshot, nameSnapshot, StringComparison.OrdinalIgnoreCase));

        if (existing is null)
            throw new DomainException<MaintenanceRecord>(PartNotFound);

        _partsUsed.Remove(existing);
        Touch();
    }

    /// <summary>
    /// Прикрепляет вложение
    /// </summary>
    public void Attach(AttachmentRef attachment)
    {
        EnsureEditable();
        if (_attachments.Any(a => a.AttachmentId == attachment.AttachmentId))
            throw new DomainException<MaintenanceRecord>(DuplicateAttachment);

        _attachments.Add(attachment);
        Touch();
    }

    /// <summary>
    /// Открепляет вложение
    /// </summary>
    public void Detach(Guid attachmentId)
    {
        EnsureEditable();
        var existing = _attachments.FirstOrDefault(a => a.AttachmentId == attachmentId)
            ?? throw new DomainException<MaintenanceRecord>(AttachmentNotFound);
        _attachments.Remove(existing);
        Touch();
    }

    /// <summary>
    /// Привязывает покупку
    /// </summary>
    public void LinkPurchase(Guid purchaseId)
    {
        EnsureEditable();
        if (purchaseId == default(Guid))
            throw new DomainException<MaintenanceRecord>(PurchaseIdCanNotBeDefault);

        if (_purchaseLinks.Any(l => l.PurchaseId == purchaseId))
            return;

        _purchaseLinks.Add(MaintenancePurchaseLink.Create(purchaseId));
        Touch();

        AddDomainEvent(new PurchaseLinkedToMaintenance(Id, purchaseId));
    }

    /// <summary>
    /// Отвязывает покупку
    /// </summary>
    public void UnlinkPurchase(Guid purchaseId)
    {
        EnsureEditable();
        var existing = _purchaseLinks.FirstOrDefault(l => l.PurchaseId == purchaseId);
        if (existing is null)
            return;
        _purchaseLinks.Remove(existing);
        Touch();
        AddDomainEvent(new PurchaseUnlinkedFromMaintenance(Id, purchaseId));
    }
    
    public void Start()
    {
        TransitionTo(MaintenanceStatus.InProgress, new[] { MaintenanceStatus.Planned });
        AddDomainEvent(new MaintenanceRecordStarted(Id));
    }

    public void Complete()
    {
        TransitionTo(MaintenanceStatus.Completed, new[] { MaintenanceStatus.Planned, MaintenanceStatus.InProgress });
        AddDomainEvent(new MaintenanceRecordCompleted(Id,
            VehicleId,
            TaskType.TaskTypeId,
            PerformedAt,
            OdometerAtService.Kilometers,
            LaborCost.Amount,
            LaborCost.Currency));
    }

    public void Cancel(string reason)
    {
        EnsureNotTerminal();
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException<MaintenanceRecord>(CancellationReasonIsRequired);
        Status = MaintenanceStatus.Cancelled;
        Touch();
        AddDomainEvent(new MaintenanceRecordCancelled(Id, reason));
    }

    private void TransitionTo(MaintenanceStatus target, MaintenanceStatus[] allowedFrom)
    {
        if (!allowedFrom.Contains(Status))
            throw new DomainException<MaintenanceRecord>(InvalidStatusTransition);

        Status = target;
        Touch();
    }

    private void EnsureEditable()
    {
        if (Status is MaintenanceStatus.Completed or MaintenanceStatus.Cancelled)
            throw new DomainException<MaintenanceRecord>(RecordIsNotEditable);
    }

    private void EnsureNotTerminal()
    {
        if (Status is MaintenanceStatus.Completed or MaintenanceStatus.Cancelled)
            throw new DomainException<MaintenanceRecord>(RecordInTerminalStatus);
    }

    private static void EnsureDateNotFuture(DateTimeOffset date)
    {
        if (date > DateTimeOffset.UtcNow.AddMinutes(5))
            throw new DomainException<MaintenanceRecord>(RecordFutureDate);
    }

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}
