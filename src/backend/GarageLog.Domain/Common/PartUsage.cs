using GarageLog.Domain.Infrastrucuture;
using GarageLog.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;
using static GarageLog.Domain.Errors.PartUsageErrors;

namespace GarageLog.Domain.Common;

public sealed class PartUsage : ValueObject
{
    public Guid? PartId { get; }
    public string NameSnapshot { get; }
    public decimal Quantity { get; }
    public PartUnit Unit { get; }

    private PartUsage(Guid? partId, string nameSnapshot, decimal quantity, PartUnit unit)
    {
        PartId = partId;
        NameSnapshot = nameSnapshot;
        Quantity = quantity;
        Unit = unit;
    }

    public static PartUsage Create(Guid? partId, string name, decimal quantity, PartUnit unit = PartUnit.Piece)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException<PartUsage>(PartUsageNameIsEmpty);
        if (quantity <= 0)
            throw new DomainException<PartUsage>(PartUsageInvalidQuantity);
        return new PartUsage(partId, name.Trim(), decimal.Round(quantity, 3), unit);
    }

    public override string ToString() => $"{NameSnapshot} × {Quantity} {Unit}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PartId;
        yield return NameSnapshot;
        yield return Unit;
        yield return Quantity;
    }
}
