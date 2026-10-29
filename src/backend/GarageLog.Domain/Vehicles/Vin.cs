using GarageLog.Domain.Infrastrucuture;
using GarageLog.SharedKernel;
using static GarageLog.Domain.Errors.VinErrors;

namespace GarageLog.Domain.Vehicles;

/// <summary>
/// VIN номер
/// </summary>
public sealed class Vin : ValueObject
{
    /// <summary>
    /// Значение VIN
    /// </summary>
    public string Value { get; }
    private static readonly HashSet<char> ForbiddenChars = new() { 'I', 'O', 'Q' };

    private Vin(string value) => Value = value;

    /// <summary>
    /// Создает VIN номер из входящей строки
    /// </summary>
    public static Vin From(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new DomainException<Vin>(VinIsEmpty);
        string normilized = input.Trim().ToUpperInvariant();
        if (normilized.Length != 17)
            throw new DomainException<Vin>(VinInvalidLength);
        if (normilized.Any(c => ForbiddenChars.Contains(c)))
            throw new DomainException<Vin>(VinForbiddenChars);
        if (!normilized.All(c => char.IsAsciiLetterOrDigit(c)))
            throw new DomainException<Vin>(VinInvalidChars);
        return new Vin(input);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}