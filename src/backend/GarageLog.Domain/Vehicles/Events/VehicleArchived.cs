using GarageLog.SharedKernel;

namespace GarageLog.Domain.Vehicles.Events;

public class VehicleArchived(Guid vehicleId) : DomainEvent
{
    public Guid VehicleId { get; } = vehicleId;
}
