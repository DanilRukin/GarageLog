using GarageLog.Domain.Infrastrucuture;
using GarageLog.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;
using static GarageLog.Domain.Errors.LicensePlateErrors;

namespace GarageLog.Domain.Vehicles;

/// <summary>
/// Номерной знак
/// </summary>
public sealed class LicensePlate : ValueObject
{
    /// <summary>
    /// Значение номера
    /// </summary>
    public string Value { get; }
    private LicensePlate(string value) => Value = value;

    public static LicensePlate From(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new DomainException<LicensePlate>(LicensePlateIsEmpty);
        string normalised = input.Trim().ToUpperInvariant().Replace(" ", "");
        if (normalised.Length is < 3 or > 12)
            throw new DomainException<LicensePlate>(LicensePlateInvalidLength);
        return new LicensePlate(input);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
