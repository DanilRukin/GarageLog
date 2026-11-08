using GarageLog.Domain.Infrastructure;
using GarageLog.Domain.Planning.Maintenance;
using GarageLog.Domain.Planning.Triggers;
using static GarageLog.Domain.Errors.NextDueCalculatorErrors;

namespace GarageLog.Domain.Planning.Services;

/// <summary>
/// Вычисляет следующий срок срабатывания для <see cref="PlannedMaintenance"/>
/// на основе его триггера, last-значений и текущего времени.
/// </summary>
public class NextDueCalculator
{
    private readonly TimeProvider _time;

    /// <summary>Создаёт калькулятор с указанным источником времени (для тестируемости).</summary>
    public NextDueCalculator(TimeProvider time) => _time = time;

    /// <summary>
    /// Вычисляет следующий срок для плана.
    /// </summary>
    /// <param name="plan">Плановая работа.</param>
    /// <returns>Срок по дате и/или пробегу. Может быть <c>null</c> в обоих полях, если план терминальный.</returns>
    public NextDue Compute(PlannedMaintenance plan)
    {
        if (plan.Status is PlannedMaintenanceStatus.Done
            or PlannedMaintenanceStatus.Skipped
            or PlannedMaintenanceStatus.Cancelled)
            return new NextDue(null, null);
        return plan.Trigger switch
        {
            ByDateTrigger d => new NextDue(d.DueDate, null),
            ByOdometerTrigger b => new NextDue(null, b.DueOdometer),
            ByIntervalTrigger i => ComputeInterval(plan, i),
            CombinedTrigger c => ComputeCombined(plan, c),
            _ => throw new DomainException<NextDueCalculator>(UnknownTrigger)
        };
    }

    /// <summary>
    /// Вычисляет срок для интервального триггера: last + интервал.
    /// </summary>
    private NextDue ComputeInterval(PlannedMaintenance plan, ByIntervalTrigger i)
    {
        DateTimeOffset? date = null;
        if (plan.LastPerformedAt is not null && i.Months is not null)
            date = plan.LastPerformedAt.Value.AddMonths(i.Months.Months);
        int? odo = null;
        if (plan.LastOdometer is not null && i.Km is not null)
            odo = plan.LastOdometer.Value + i.Km.Kilometers;
        return new NextDue(date, odo);
    }

    /// <summary>
    /// Вычисляет срок для комбинированного триггера: берёт самый ранний из составляющих.
    /// </summary>
    private NextDue ComputeCombined(PlannedMaintenance plan, CombinedTrigger c)
    {
        List<NextDue> candidates = new();

        if (c.Date is not null) candidates.Add(new NextDue(c.Date.DueDate, null));
        if (c.Odometer is not null) candidates.Add(new NextDue(null, c.Odometer.DueOdometer));
        if (c.Interval is not null) candidates.Add(ComputeInterval(plan, c.Interval));

        // «Что раньше»: по дате — минимальная дата; по пробегу — минимальный пробег.
        // Если есть и дата, и пробег — оба считаются независимыми триггерами.
        var minDate = candidates.Where(x => x.DueDate is not null).Min(x => x.DueDate);
        var minOdometer = candidates.Where(x => x.DueOdometer is not null).Min(x => x.DueOdometer);

        return new NextDue(minDate, minOdometer);
    }
}