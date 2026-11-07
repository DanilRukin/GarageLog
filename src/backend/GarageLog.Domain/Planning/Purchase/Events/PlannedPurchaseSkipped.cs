using GarageLog.SharedKernel;

namespace GarageLog.Domain.Planning.Purchase.Events;

/// <summary>
/// План покупки пропущен.
/// </summary>
/// <param name="plannedPurchaseId">Идентификатор плана.</param>
/// <param name="reason">Причина пропуска (может быть null).</param>
public class PlannedPurchaseSkipped(
    Guid plannedPurchaseId,
    string? reason) : DomainEvent
{
    public Guid PlannedPurchaseId { get; } = plannedPurchaseId;
    public string? Reason { get; } = reason;
}
