using GarageLog.Domain.Infrastrucuture;
using GarageLog.SharedKernel;
using static GarageLog.Domain.Errors.OdometerIntervalErrors;

namespace GarageLog.Domain.Planning;

/// <summary>
/// Интервал пробега
/// </summary>
public sealed class OdometerInterval : ValueObject
{
    /// <summary>
    /// Кол-во километров в интервале
    /// </summary>
    public int Kilometers { get; }
    private OdometerInterval(int kilometers) => Kilometers = kilometers;

    public static OdometerInterval Of(int kilometers)
    {
        if (kilometers <= 0)
            throw new DomainException<OdometerInterval>(IntervalIsNotPositive);
        return new OdometerInterval(kilometers);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Kilometers;
    }

    public override string ToString() => $"{Kilometers:N0}";
}
