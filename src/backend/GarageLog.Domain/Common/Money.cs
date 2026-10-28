using GarageLog.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GarageLog.Domain.Common;

public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; } = default!;
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    private Money(decimal amount,  string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Zero(string currrency) => new Money(0m, currrency);
}