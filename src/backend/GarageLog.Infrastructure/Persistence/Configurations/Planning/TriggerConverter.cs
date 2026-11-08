using GarageLog.Domain.Planning;
using GarageLog.Domain.Planning.Triggers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static GarageLog.Infrastructure.Persistence.Configurations.Planning.TriggerKinds;

namespace GarageLog.Infrastructure.Persistence.Configurations.Planning;

public class TriggerConverter : ValueConverter<Trigger, TriggerRecord>
{
    public TriggerConverter() : base(
        t => ToRecord(t),
        r => FromRecord(r))
    { }

    private static TriggerRecord ToRecord(Trigger t) => t switch
    {
        ByDateTrigger d => new() { Kind = ByDate, DueDate = d.DueDate },
        ByOdometerTrigger o => new() { Kind = ByOdometer, DueOdometer = o.DueOdometer },
        ByIntervalTrigger i => new()
        {
            Kind = ByInterval,
            IntervalKm = i.Km?.Kilometers,
            IntervalMonths = i.Months?.Months
        },
        CombinedTrigger c => new()
        {
            Kind = Combined,
            CombinedDate = c.Date?.DueDate,
            CombinedOdometer = c.Odometer?.DueOdometer,
            CombinedIntervalKm = c.Interval?.Km?.Kilometers,
            CombinedIntervalMonths = c.Interval?.Months?.Months
        },
        _ => throw new InvalidOperationException($"Unknown trigger: {t.GetType()}")
    };

    private static Trigger FromRecord(TriggerRecord r) => r.Kind switch
    {
        ByDate => new ByDateTrigger(r.DueDate!.Value),
        ByOdometer => new ByOdometerTrigger(r.DueOdometer!.Value),
        ByInterval => new ByIntervalTrigger(
            r.IntervalKm is null ? null : OdometerInterval.Of(r.IntervalKm.Value),
            r.IntervalMonths is null ? null : TimeInterval.OfMonths(r.IntervalMonths.Value)),
        Combined => new CombinedTrigger(
            r.CombinedDate is null ? null : new ByDateTrigger(r.CombinedDate.Value),
            r.CombinedOdometer is null ? null : new ByOdometerTrigger(r.CombinedOdometer.Value),
            r.CombinedIntervalKm is null && r.CombinedIntervalMonths is null
                ? null
                : new ByIntervalTrigger(
                    r.CombinedIntervalKm is null ? null : OdometerInterval.Of(r.CombinedIntervalKm.Value),
                    r.CombinedIntervalMonths is null ? null : TimeInterval.OfMonths(r.CombinedIntervalMonths.Value))),
        _ => throw new InvalidOperationException($"Unknown kind: {r.Kind}")
    };
}
