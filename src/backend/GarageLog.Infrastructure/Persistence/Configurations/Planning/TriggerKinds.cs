namespace GarageLog.Infrastructure.Persistence.Configurations.Planning;

/// <summary>
/// Имена типов триггеров (для БД)
/// </summary>
public static class TriggerKinds
{
    public const string ByDate = "ByDate";
    public const string ByOdometer = "ByOdometer";
    public const string ByInterval = "ByInterval";
    public const string Combined = "Combined";
}
