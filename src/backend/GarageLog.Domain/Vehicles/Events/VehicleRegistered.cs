using GarageLog.SharedKernel;

namespace GarageLog.Domain.Vehicles.Events;

public class VehicleRegistered(Guid vehicleId, Guid ownerId, string vin, string? licensePlate) : DomainEvent
{
    public Guid VehicleId { get; } = vehicleId;
    public Guid OwnerId { get; } = ownerId;
    public string Vin { get; } = vin;
    public string? LicensePlate { get; } = licensePlate;
}
