using GarageLog.Domain.Common;
using GarageLog.Domain.Infrastructure;
using GarageLog.Domain.Planning.Purchase.Events;
using GarageLog.SharedKernel;
using GarageLog.SharedKernel.Interfaces;
using static GarageLog.Domain.Errors.PlannedPurchaseErrors;

namespace GarageLog.Domain.Planning.Purchase;

/// <summary>
/// План покупки запчасти/расходника. Может быть привязан к автомобилю или быть «общим».
/// При выполнении ссылается на созданную <c>Purchase</c> по идентификатору.
/// </summary>
public class PlannedPurchase : EntityBase<Guid>, IAgregateRoot
{
    /// <summary>Идентификатор автомобиля, к которому относится план. Null — «общий» план.</summary>
    public Guid? VehicleId { get; private init; }

    /// <summary>Ссылка на запчасть в справочнике (если есть). Может быть null для свободного текста.</summary>
    public Guid? PartId { get; private set; }

    /// <summary>Наименование планируемой покупки. Снапшот на момент создания/редактирования.</summary>
    public string Name { get; private set; } = default!;

    /// <summary>Планируемое количество.</summary>
    public decimal Quantity { get; private set; }

    /// <summary>Единица измерения количества.</summary>
    public PartUnit Unit { get; private set; }

    /// <summary>Оценочная цена за единицу. Может быть null, если пользователь ещё не решил.</summary>
    public Money? EstimatedPrice { get; private set; }

    /// <summary>Планируемая дата покупки. Null — «когда получится».</summary>
    public DateTimeOffset? PlannedForDate { get; private set; }

    /// <summary>Приоритет плана.</summary>
    public PlanningPriority Priority { get; private set; }

    /// <summary>Текущий статус плана.</summary>
    public PlannedPurchaseStatus Status { get; private set; }

    /// <summary>Ссылка на созданную покупку после выполнения плана. Null до выполнения.</summary>
    public Guid? LinkedPurchaseId { get; private set; }

    /// <summary>Свободный комментарий к плану.</summary>
    public string? Notes { get; private set; }

    /// <summary>Момент создания агрегата (UTC).</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Момент последнего изменения агрегата (UTC).</summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>Конструктор для EF Core.</summary>
    private PlannedPurchase() { }

    /// <summary>
    /// Создаёт новый план покупки.
    /// </summary>
    /// <param name="vehicleId">Идентификатор автомобиля или null для «общего» плана.</param>
    /// <param name="name">Наименование планируемой покупки.</param>
    /// <param name="quantity">Количество (&gt; 0).</param>
    /// <param name="unit">Единица измерения.</param>
    /// <param name="partId">Идентификатор запчасти в справочнике (опционально).</param>
    /// <param name="estimatedPrice">Оценочная цена за единицу (опционально).</param>
    /// <param name="plannedForDate">Планируемая дата покупки (опционально).</param>
    /// <param name="priority">Приоритет; по умолчанию <see cref="PlanningPriority.Normal"/>.</param>
    /// <param name="notes">Комментарий.</param>
    /// <returns>Новый агрегат <see cref="PlannedPurchase"/>.</returns>
    public static PlannedPurchase Create(
        Guid? vehicleId,
        string name,
        decimal quantity,
        PartUnit unit = PartUnit.Piece,
        Guid? partId = null,
        Money? estimatedPrice = null,
        DateTimeOffset? plannedForDate = null,
        PlanningPriority priority = PlanningPriority.Normal,
        string? notes = null)
    {
        EnsureNameIsNotEmpty(name);
        EnsurePositiveQuantity(quantity);
        EnsurePlannedDateNotPast(plannedForDate);
        var now = DateTimeOffset.UtcNow;
        var plan = new PlannedPurchase
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicleId == Guid.Empty ? null : vehicleId,
            PartId = partId,
            Name = name.Trim(),
            Quantity = decimal.Round(quantity, 3),
            Unit = unit,
            EstimatedPrice = estimatedPrice,
            PlannedForDate = plannedForDate,
            Priority = priority,
            Status = PlannedPurchaseStatus.Planned,
            Notes = notes?.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };
        plan.AddDomainEvent(new PlannedPurchaseCreated(
            plan.Id, plan.VehicleId, plan.Name, plan.PlannedForDate));

        return plan;
    }

    /// <summary>
    /// Обновляет данные плана. Доступно только в статусе <see cref="PlannedPurchaseStatus.Planned"/>.
    /// </summary>
    /// <param name="name">Новое наименование.</param>
    /// <param name="quantity">Новое количество (&gt; 0).</param>
    /// <param name="unit">Единица измерения.</param>
    /// <param name="partId">Ссылка на справочник (опционально).</param>
    /// <param name="estimatedPrice">Оценочная цена (опционально).</param>
    /// <param name="plannedForDate">Планируемая дата (опционально).</param>
    /// <param name="priority">Приоритет.</param>
    /// <param name="notes">Комментарий.</param>
    public void UpdateDetails(
        string name,
        decimal quantity,
        PartUnit unit,
        Guid? partId,
        Money? estimatedPrice,
        DateTimeOffset? plannedForDate,
        PlanningPriority priority,
        string? notes)
    {
        EnsureEditable();
        EnsureNameIsNotEmpty(name);
        EnsurePositiveQuantity(quantity);
        EnsurePlannedDateNotPast(plannedForDate);

        Name = name.Trim();
        Quantity = decimal.Round(quantity, 3);
        Unit = unit;
        PartId = partId;
        EstimatedPrice = estimatedPrice;
        PlannedForDate = plannedForDate;
        Priority = priority;
        Notes = notes?.Trim();
        Touch();
    }

    /// <summary>
    /// Изменяет только приоритет плана. Доступно в статусе <see cref="PlannedPurchaseStatus.Planned"/>.
    /// </summary>
    /// <param name="priority">Новый приоритет.</param>
    public void ChangePriority(PlanningPriority priority)
    {
        EnsureEditable();
        Priority = priority;
        Touch();
    }

    /// <summary>
    /// Откладывает план на новую дату. Доступно в статусе <see cref="PlannedPurchaseStatus.Planned"/>.
    /// </summary>
    /// <param name="newDate">Новая планируемая дата (или null — «когда получится»).</param>
    public void Reschedule(DateTimeOffset? newDate)
    {
        EnsureEditable();
        EnsurePlannedDateNotPast(newDate);

        PlannedForDate = newDate;
        Touch();
    }

    /// <summary>
    /// Помечает план как выполненный: пользователь купил товар и создал <c>Purchase</c>.
    /// </summary>
    /// <param name="purchaseId">Идентификатор созданной покупки.</param>
    public void Fulfill(Guid purchaseId)
    {
        if (Status != PlannedPurchaseStatus.Planned)
            throw new DomainException<PlannedPurchase>(FulFillPurchaseOnlyInPlannedStatus);

        if (purchaseId == default(Guid))
            throw new DomainException<PlannedPurchase>(CreatedPurchaseIdIsRequired);

        Status = PlannedPurchaseStatus.Done;
        LinkedPurchaseId = purchaseId;
        Touch();

        AddDomainEvent(new PlannedPurchaseFulfilled(Id, purchaseId));
    }

    /// <summary>
    /// Пропускает план: пользователь решил не покупать, без оформления как отмены.
    /// </summary>
    /// <param name="reason">Причина пропуска (опционально).</param>
    public void Skip(string? reason = null)
    {
        EnsurePlanned();

        Status = PlannedPurchaseStatus.Skipped;
        Touch();

        AddDomainEvent(new PlannedPurchaseSkipped(Id, reason?.Trim()));
    }

    /// <summary>
    /// Отменяет план.
    /// </summary>
    /// <param name="reason">Причина отмены.</param>
    public void Cancel(string reason)
    {
        EnsurePlanned();
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException<PlannedPurchase>(CancellationReasonIsRequired);

        Status = PlannedPurchaseStatus.Cancelled;
        Touch();

        AddDomainEvent(new PlannedPurchaseCancelled(Id, reason.Trim()));
    }

    /// <summary>Проверяет, что план находится в статусе <see cref="PlannedPurchaseStatus.Planned"/>.</summary>
    private void EnsurePlanned()
    {
        if (Status != PlannedPurchaseStatus.Planned)
            throw new DomainException<PlannedPurchase>(OperationAvailableOnlyInPlannedStatus);
    }

    /// <summary>Проверяет, что план редактируем (статус Planned).</summary>
    private void EnsureEditable() => EnsurePlanned();

    /// <summary>Проверяет, что количество положительное.</summary>
    private static void EnsurePositiveQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException<PlannedPurchase>(QualityIsNotPositive);
    }

    /// <summary>
    /// Проверяет, что планируемая дата не в прошлом.
    /// </summary>
    private static void EnsurePlannedDateNotPast(DateTimeOffset? date)
    {
        if (date is { } d && d < DateTimeOffset.UtcNow.AddMinutes(-1))
            throw new DomainException<PlannedPurchase>(PlannedDateIsInPast);
    }

    /// <summary>
    /// Проверяет, что название покупи не пустое
    /// </summary>
    /// <param name="name">Название планируемой покупки</param>
    private static void EnsureNameIsNotEmpty(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException<PlannedPurchase>(PurchaseNameIsEmpty);
    }

    /// <summary>Обновляет <see cref="UpdatedAt"/> текущим временем.</summary>
    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}
