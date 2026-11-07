using GarageLog.SharedKernel;

namespace GarageLog.Domain.Purchases.Events;

/// <summary>
/// Покупка возвращена (полностью).
/// </summary>
/// <param name="purchaseId">Идентификатор покупки.</param>
/// <param name="reason">Причина возврата.</param>
public class PurchaseReturned(
    Guid purchaseId,
    string reason) : DomainEvent
{
    public Guid PurchaseId { get; } = purchaseId;
    public string Reason { get; } = reason;
}

