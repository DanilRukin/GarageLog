using GarageLog.SharedKernel;

namespace GarageLog.Domain.Vehicles.Events;

public class VehicleDetailsUpdated(Guid vehicleId) : DomainEvent
{
    public Guid VehicleId { get; } = vehicleId;
}
