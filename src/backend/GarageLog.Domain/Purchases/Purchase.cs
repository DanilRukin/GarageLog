using GarageLog.Domain.Common;
using GarageLog.Domain.Infrastructure;
using GarageLog.Domain.Purchases.Events;
using GarageLog.SharedKernel;
using GarageLog.SharedKernel.Interfaces;
using static GarageLog.Domain.Errors.PurchaseErrors;

namespace GarageLog.Domain.Purchases;

/// <summary>
/// Покупка запчастей/расходников. Может быть привязана к автомобилю или быть «общей».
/// Содержит коллекцию позиций (<see cref="PurchaseLine"/>), которые являются частью агрегата.
/// </summary>
public class Purchase : EntityBase<Guid>, IAgregateRoot
{
    /// <summary>Позиции покупки. Owned-коллекция внутри агрегата.</summary>
    private readonly List<PurchaseLine> _lines = new();

    /// <summary>Идентификатор автомобиля, к которому относится покупка. Null — «общая» покупка.</summary>
    public Guid? VehicleId { get; private init; }

    /// <summary>Дата и время покупки. Может быть в будущем для предзаказов (при статусе Draft).</summary>
    public DateTimeOffset PurchasedAt { get; private set; }

    /// <summary>Продавец/магазин. Снапшот имени — не ссылка на справочник.</summary>
    public string Vendor { get; private set; } = default!;

    /// <summary>Снапшот итоговой суммы на момент последнего изменения линий.</summary>
    public Money TotalAmount { get; private set; } = default!;

    /// <summary>Ссылка на чек (вложение). Может отсутствовать.</summary>
    public AttachmentRef? Receipt { get; private set; }

    /// <summary>Произвольный комментарий к покупке.</summary>
    public string? Notes { get; private set; }

    /// <summary>Текущий статус покупки.</summary>
    public PurchaseStatus Status { get; private set; }

    /// <summary>Момент создания агрегата (UTC).</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Момент последнего изменения агрегата (UTC).</summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>Позиции покупки (только для чтения).</summary>
    public IReadOnlyCollection<PurchaseLine> Lines => _lines.AsReadOnly();

    /// <summary>Конструктор для EF Core.</summary>
    private Purchase() { }

    /// <summary>
    /// Создаёт новую покупку в статусе <see cref="PurchaseStatus.Draft"/>.
    /// </summary>
    /// <param name="vehicleId">Идентификатор автомобиля или null для «общей» покупки.</param>
    /// <param name="vendor">Название продавца/магазина.</param>
    /// <param name="currency">Код валюты ISO 4217 (например, "RUB").</param>
    /// <param name="purchasedAt">Дата покупки; по умолчанию — текущий момент.</param>
    /// <param name="notes">Произвольный комментарий.</param>
    /// <returns>Новый агрегат <see cref="Purchase"/>.</returns>
    public static Purchase Create(
        Guid? vehicleId,
        string vendor,
        string currency,
        DateTimeOffset? purchasedAt = null,
        string? notes = null)
    {
        CheckVendorIsNotEmpty(vendor);
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException<Purchase>(CurrencyCanNotBeEmpty);

        var at = purchasedAt ?? DateTimeOffset.UtcNow;
        EnsureDateNotInFuture(at);

        var now = DateTimeOffset.UtcNow;
        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicleId == Guid.Empty ? null : vehicleId,
            Vendor = vendor.Trim(),
            TotalAmount = Money.Zero(currency),
            Notes = notes?.Trim(),
            PurchasedAt = at,
            Status = PurchaseStatus.Draft,
            CreatedAt = now,
            UpdatedAt = now
        };

        purchase.AddDomainEvent(new PurchaseRegistered(
            purchase.Id, purchase.VehicleId, 0m, purchase.TotalAmount.Currency));

        return purchase;
    }

    /// <summary>
    /// Добавляет позицию в покупку. Валюта позиции должна совпадать с валютой покупки.
    /// Пересчитывает <see cref="TotalAmount"/>.
    /// </summary>
    /// <param name="line">Позиция покупки.</param>
    public void AddLine(PurchaseLine line)
    {
        EnsureEditable();
        if (line is null)
            throw new DomainException<Purchase>(PurchaseLineIsRequired);

        EnsureSameCurrency(line.UnitPrice.Currency);

        var duplicate = _lines.Any(l =>
            l.PartId == line.PartId &&
            string.Equals(l.NameSnapshot, line.NameSnapshot, StringComparison.OrdinalIgnoreCase));
        if (duplicate)
            throw new DomainException<Purchase>(PurchaseLineDuplicated);

        _lines.Add(line);
        RecalculateTotal();
        Touch();
    }

    /// <summary>
    /// Удаляет позицию из покупки.
    /// </summary>
    /// <param name="partId">Идентификатор запчасти (если была привязка к справочнику).</param>
    /// <param name="nameSnapshot">Снапшот имени позиции для точного поиска.</param>
    public void RemoveLine(Guid? partId, string nameSnapshot)
    {
        EnsureEditable();
        if (string.IsNullOrWhiteSpace(nameSnapshot))
            throw new DomainException<Purchase>(PurchaseNameSnapshotIsEmpty);

        var existing = _lines.FirstOrDefault(l =>
            l.PartId == partId &&
            string.Equals(l.NameSnapshot, nameSnapshot, StringComparison.OrdinalIgnoreCase));

        if (existing is null)
            throw new DomainException<Purchase>(PurchaseLineNotFound);

        _lines.Remove(existing);
        RecalculateTotal();
        Touch();
    }

    /// <summary>
    /// Обновляет общие данные покупки: продавца, дату, комментарий.
    /// </summary>
    /// <param name="vendor">Название продавца.</param>
    /// <param name="purchasedAt">Дата покупки.</param>
    /// <param name="notes">Комментарий.</param>
    public void UpdateDetails(string vendor, DateTimeOffset purchasedAt, string? notes)
    {
        EnsureEditable();
        CheckVendorIsNotEmpty(vendor);

        EnsureDateNotInFuture(purchasedAt);

        Vendor = vendor.Trim();
        PurchasedAt = purchasedAt;
        Notes = notes?.Trim();
        Touch();
    }

    /// <summary>
    /// Прикрепляет чек к покупке.
    /// </summary>
    /// <param name="receipt">Ссылка на вложение.</param>
    public void AttachReceipt(AttachmentRef receipt)
    {
        EnsureEditable();
        if (receipt is null)
            throw new DomainException<Purchase>(ReceiptIsRequired);

        Receipt = receipt;
        Touch();
    }

    /// <summary>
    /// Открепляет чек от покупки.
    /// </summary>
    public void DetachReceipt()
    {
        EnsureEditable();
        Receipt = null;
        Touch();
    }

    /// <summary>
    /// Подтверждает покупку. Переход Draft → Confirmed.
    /// </summary>
    /// <exception cref="DomainException">Если переход недопустим или нет ни одной позиции.</exception>
    public void Confirm()
    {
        if (Status != PurchaseStatus.Draft)
            throw new DomainException<Purchase>(InvalidTransition);

        if (_lines.Count == 0)
            throw new DomainException<Purchase>(PurchaseLineIsEmpty);

        Status = PurchaseStatus.Confirmed;
        Touch();

        AddDomainEvent(new PurchaseConfirmed(
            Id, TotalAmount.Amount, TotalAmount.Currency, PurchasedAt));
    }

    /// <summary>
    /// Помечает покупку как возвращённую (полностью).
    /// </summary>
    /// <param name="reason">Причина возврата.</param>
    /// <exception cref="DomainException">Если покупка уже возвращена.</exception>
    public void Return(string reason)
    {
        if (Status == PurchaseStatus.Returned)
            throw new DomainException<Purchase>(PurchaseAlreadyReturned);

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException<Purchase>(PurchaseReturningReasonIsRequired);

        Status = PurchaseStatus.Returned;
        Touch();

        AddDomainEvent(new PurchaseReturned(Id, reason.Trim()));
    }

    /// <summary>
    /// Пересчитывает <see cref="TotalAmount"/> как сумму всех линий.
    /// </summary>
    private void RecalculateTotal()
    {
        var currency = TotalAmount.Currency;
        var total = Money.Zero(currency);
        foreach (var line in _lines)
            total += line.LineTotal;
        TotalAmount = total;
    }

    /// <summary>
    /// Проверяет, что валюта совпадает с валютой покупки.
    /// </summary>
    private void EnsureSameCurrency(string currency)
    {
        if (!string.Equals(TotalAmount.Currency, currency, StringComparison.Ordinal))
            throw new DomainException<Purchase>(PurchaseCurrencyMismatch);
    }

    /// <summary>
    /// Проверяет, что покупка находится в редактируемом статусе (Draft).
    /// </summary>
    private void EnsureEditable()
    {
        if (Status != PurchaseStatus.Draft)
            throw new DomainException<Purchase>(PurchaseIsNotEditable);
    }

    /// <summary>Обновляет <see cref="UpdatedAt"/> текущим временем.</summary>
    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;

    /// <summary>
    /// Проверяет, что вендор не пуст
    /// </summary>
    /// <param name="vendor">Имя продавца</param>
    private static void CheckVendorIsNotEmpty(string vendor)
    {
        if (string.IsNullOrWhiteSpace(vendor))
            throw new DomainException<Purchase>(VendorCanNotBeEmpty);
    }

    /// <summary>
    /// Проверяет, что дата покупки не в будущем
    /// </summary>
    /// <param name="date">Дата покупки</param>
    private static void EnsureDateNotInFuture(DateTimeOffset date)
    {
        if (date > DateTimeOffset.UtcNow.AddMinutes(5))
            throw new DomainException<Purchase>(PurchaseCanNotBeInTheFuture);
    }
}
