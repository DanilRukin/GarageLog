using GarageLog.SharedKernel;

namespace GarageLog.Domain.Planning.Purchase.Events;

/// <summary>
/// План покупки помечен как выполненный (куплено).
/// </summary>
/// <param name="plannedPurchaseId">Идентификатор плана.</param>
/// <param name="purchaseId">Идентификатор созданной покупки.</param>
public class PlannedPurchaseFulfilled(
    Guid plannedPurchaseId,
    Guid purchaseId) : DomainEvent
{
    public Guid PlannedPurchaseId { get; } = plannedPurchaseId;
    public Guid PurchaseId { get; } = purchaseId;
}
