using GarageLog.Domain.Infrastructure;
using GarageLog.SharedKernel;
using static GarageLog.Domain.Errors.OdometerReadingErrors;

namespace GarageLog.Domain.Vehicles;

/// <summary>
/// Показание пробега
/// </summary>
public sealed class OdometerReading : ValueObject
{
    /// <summary>
    /// Километраж
    /// </summary>
    public int Kilometers { get; }
    /// <summary>
    /// Дата получения километража
    /// </summary>
    public DateTimeOffset TakenAt { get; }
    /// <summary>
    /// Источник получения километража
    /// </summary>
    public OdometerSource Source { get; }

    private OdometerReading(int kilometers, DateTimeOffset takenAt, OdometerSource source)
    {
        Kilometers = kilometers;
        TakenAt = takenAt;
        Source = source;
    }

    public static OdometerReading Of(int kilometers, DateTimeOffset takenAt, 
        OdometerSource source = OdometerSource.Manual)
    {
        if (kilometers < 0)
            throw new DomainException<OdometerReading>(OdometerReadingIsNegative);
        if (takenAt > DateTimeOffset.UtcNow.AddMinutes(5))
            throw new DomainException<OdometerReading>(OdometerReadingsInTheFuture);
        return new OdometerReading(kilometers, takenAt, source);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Kilometers;
        yield return TakenAt;
        yield return Source;
    }

    public override string ToString() => $"{Kilometers:N0} км ({TakenAt:yyyy-MM-dd}, {Source})";
}
