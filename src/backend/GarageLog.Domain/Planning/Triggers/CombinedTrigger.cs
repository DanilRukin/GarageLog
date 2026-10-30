using GarageLog.Domain.Infrastrucuture;
using static GarageLog.Domain.Errors.CombinedTriggerErrors;

namespace GarageLog.Domain.Planning.Triggers;

/// <summary>
/// Комбинированный триггер (под вопросом)
/// </summary>
public sealed record CombinedTrigger : Trigger
{
    public ByDateTrigger? Date { get; }
    public ByOdometerTrigger? Odometer { get; }
    public ByIntervalTrigger? Interval { get; }
    public CombinedTrigger(
        ByDateTrigger? date,
        ByOdometerTrigger? odometer,
        ByIntervalTrigger? interval)
    {
        Date = date;
        Odometer = odometer;
        Interval = interval;
        if (Date is null && Odometer is null && Interval is null)
            throw new DomainException<CombinedTrigger>(CombinedTriggerIsEmpty);
    }

    public override string Describe()
    {
        var parts = new List<string>();
        if (Date is not null) parts.Add(Date.Describe());
        if (Odometer is not null) parts.Add(Odometer.Describe());
        if (Interval is not null) parts.Add(Interval.Describe());
        return string.Join("; ", parts);
    }
}
