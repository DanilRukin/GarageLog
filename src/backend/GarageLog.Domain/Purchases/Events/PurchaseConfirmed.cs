using GarageLog.SharedKernel;

namespace GarageLog.Domain.Purchases.Events;

/// <summary>
/// Покупка подтверждена: зафиксирована как совершённая.
/// </summary>
/// <param name="purchaseId">Идентификатор покупки.</param>
/// <param name="totalAmount">Итоговая сумма.</param>
/// <param name="currency">Валюта покупки.</param>
/// <param name="purchasedAt">Дата совершения покупки.</param>
public class PurchaseConfirmed(
    Guid purchaseId,
    decimal totalAmount,
    string currency,
    DateTimeOffset purchasedAt) : DomainEvent
{
    public Guid PurchaseId { get; } = purchaseId;
    public decimal TotalAmount { get; } = totalAmount;
    public string Currency { get; } = currency;
    public DateTimeOffset PurchasedAt { get; } = purchasedAt;
}
