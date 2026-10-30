using GarageLog.Domain.Infrastrucuture;
using static GarageLog.Domain.Errors.ByIntervalTriggerErrors;

namespace GarageLog.Domain.Planning.Triggers;

public sealed record ByIntervalTrigger : Trigger
{
    public OdometerInterval? Km { get; init; }
    public TimeInterval? Months { get; init; }

    public ByIntervalTrigger(OdometerInterval? km, TimeInterval? months)
    {
        if (km is null && months is null)
            throw new DomainException<ByIntervalTrigger>(IntervalTriggerHasEmptyInterval);

        Km = km;
        Months = months;
    }

    public override string Describe()
    {
        var parts = new List<string>();
        if (Km is not null) parts.Add(Km.ToString());
        if (Months is not null) parts.Add(Months.ToString());
        return string.Join(" или ", parts);
    }
}
