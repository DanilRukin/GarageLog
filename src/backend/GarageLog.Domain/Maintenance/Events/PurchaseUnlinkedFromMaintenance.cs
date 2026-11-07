using GarageLog.SharedKernel;

namespace GarageLog.Domain.Maintenance.Events;

public class PurchaseUnlinkedFromMaintenance(
    Guid recordId,
    Guid purchaseId) : DomainEvent
{
    public Guid RecordId { get; } = recordId;
    public Guid PurchaseId { get; } = purchaseId;
}