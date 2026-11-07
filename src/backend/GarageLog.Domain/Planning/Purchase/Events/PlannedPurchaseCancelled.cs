using GarageLog.SharedKernel;

namespace GarageLog.Domain.Planning.Purchase.Events;

/// <summary>
/// План покупки отменён.
/// </summary>
/// <param name="plannedPurchaseId">Идентификатор плана.</param>
/// <param name="reason">Причина отмены.</param>
public class PlannedPurchaseCancelled(
    Guid plannedPurchaseId,
    string reason) : DomainEvent
{
    public Guid PlannedPurchaseId { get; } = plannedPurchaseId;
    public string Reason { get; } = reason;
}
