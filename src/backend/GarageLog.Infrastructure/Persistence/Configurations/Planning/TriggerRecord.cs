namespace GarageLog.Infrastructure.Persistence.Configurations.Planning;

// Промежуточный тип для EF
public class TriggerRecord
{
    public string Kind { get; set; } = default!;
    public DateTimeOffset? DueDate { get; set; }
    public int? DueOdometer { get; set; }
    public int? IntervalKm { get; set; }
    public int? IntervalMonths { get; set; }
    public DateTimeOffset? CombinedDate { get; set; }
    public int? CombinedOdometer { get; set; }
    public int? CombinedIntervalKm { get; set; }
    public int? CombinedIntervalMonths { get; set; }
}
