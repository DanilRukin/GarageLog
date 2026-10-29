namespace GarageLog.Domain.Planning.Triggers;

public sealed record ByDateTrigger(DateTimeOffset DueDate) : Trigger
{
    public override string Describe() => $"до {DueDate:yyyy-MM-dd}";
}
