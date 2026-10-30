using GarageLog.SharedKernel;

namespace GarageLog.Domain.Vehicles.Events;

public class OdometerUpdated(
    Guid vehicleId,
    int previousKilometers,
    int newKilometers,
    OdometerSource source,
    bool isCorrection,
    string? correctionReason,
    DateTimeOffset takenAt) : DomainEvent
{
    public Guid VehicleId { get; } = vehicleId;
    public int PreviousKilometers { get; } = previousKilometers;
    public int NewKilometers { get; } = newKilometers;
    public OdometerSource Source { get; } = source;
    public bool IsCorrection { get; } = isCorrection;
    public string? CorrectionReason { get; } = correctionReason;
    public DateTimeOffset TakenAt { get; } = takenAt;
}

