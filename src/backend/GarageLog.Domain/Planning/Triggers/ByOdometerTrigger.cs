namespace GarageLog.Domain.Planning.Triggers;

public sealed record ByOdometerTrigger(int DueOdometer) : Trigger
{
    public override string Describe() => $"при пробеге {DueOdometer:N0} км";
}
