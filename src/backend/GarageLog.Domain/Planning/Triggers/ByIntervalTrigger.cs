using GarageLog.Domain.Infrastrucuture;
using System;
using System.Collections.Generic;
using System.Text;

namespace GarageLog.Domain.Planning.Triggers;

public sealed record ByIntervalTrigger : Trigger
{
    public ByIntervalTrigger(OdometerInterval? Km, TimeInterval? Months)
    {
        if (Km is null && Months is null)
            throw new DomainException("trigger.empty_interval", "Интервал-триггер должен содержать хотя бы один интервал (км или месяцы).");
    }
    public override string Describe()
    {
        throw new NotImplementedException();
    }
}
