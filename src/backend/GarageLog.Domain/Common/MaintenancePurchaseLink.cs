using GarageLog.Domain.Infrastructure;
using GarageLog.SharedKernel;
using static GarageLog.Domain.Errors.MaintenancePurchaseLink;

namespace GarageLog.Domain.Common;

/// <summary>
/// Связь записи о работе с покупкой (owned entity внутри агрегата MaintenanceRecord).
/// </summary>
public sealed class MaintenancePurchaseLink : ValueObject
{
    public Guid PurchaseId { get; private init; }
    public DateTimeOffset LinkedAt { get; private init; }

    private MaintenancePurchaseLink() { }

    private MaintenancePurchaseLink(Guid purchaseId, DateTimeOffset linkedAt)
    {
        PurchaseId = purchaseId;
        LinkedAt = linkedAt;
    }

    public static MaintenancePurchaseLink Create(Guid purchaseId, DateTimeOffset? linkedAt = null)
    {
        if (purchaseId == default(Guid))
            throw new DomainException<MaintenancePurchaseLink>(PurchaseIdIsRequired);
        return new MaintenancePurchaseLink(purchaseId, linkedAt ?? DateTimeOffset.UtcNow);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PurchaseId;
        yield return LinkedAt;
    }
}
