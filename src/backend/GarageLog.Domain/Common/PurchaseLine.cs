using GarageLog.Domain.Infrastrucuture;
using GarageLog.SharedKernel;
using static GarageLog.Domain.Errors.PurchaseLineErrors;

namespace GarageLog.Domain.Common;

public sealed class PurchaseLine : ValueObject
{
    public Guid? PartId { get; }
    public string NameSnapshot { get; }
    public decimal Quantity { get; }
    public Money UnitPrice { get; }
    public Money LineTotal => UnitPrice * Quantity;

    private PurchaseLine(Guid? partId, string nameSnapshot, decimal quantity, Money unitPrice)
    {
        PartId = partId;
        NameSnapshot = nameSnapshot;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
    public static PurchaseLine Create(Guid? partId, string name, decimal quantity, Money unitPrice)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException<PurchaseLine>(PurchaseLineNameIsEmpty);
        if (quantity <= 0)
            throw new DomainException<PurchaseLine>(PurchaseLineInvalidQuantity);
        return new PurchaseLine(partId, name.Trim(), decimal.Round(quantity, 3), unitPrice);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PartId;
        yield return NameSnapshot;
        yield return Quantity;
        yield return UnitPrice;
    }
}
