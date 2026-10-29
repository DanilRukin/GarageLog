namespace GarageLog.Domain.Vehicles;

/// <summary>
/// Источник километража
/// </summary>
public enum OdometerSource
{
    Manual = 1,
    FromService = 2,
    FromPlan = 3,
    Imported = 4,
    Correction = 5
}
