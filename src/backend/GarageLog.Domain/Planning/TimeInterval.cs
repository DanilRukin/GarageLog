using GarageLog.Domain.Infrastructure;
using GarageLog.SharedKernel;
using static GarageLog.Domain.Errors.TimeIntervalErrors;

namespace GarageLog.Domain.Planning;

/// <summary>
/// Интервал времени
/// </summary>
public sealed class TimeInterval : ValueObject
{
    /// <summary>
    /// Месяцы
    /// </summary>
    public int Months { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Months;
    }
    private TimeInterval(int months) => Months = months;

    public static TimeInterval OfMonths(int months)
    {
        if (months <= 0)
            throw new DomainException<TimeInterval>(IntervalMonthsNonPositive);
        return new TimeInterval(months);
    }

    public override string ToString() => $"{Months}";
}
