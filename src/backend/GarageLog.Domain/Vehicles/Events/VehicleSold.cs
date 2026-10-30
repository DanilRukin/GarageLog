using GarageLog.SharedKernel;

namespace GarageLog.Domain.Vehicles.Events;

public class VehicleSold(Guid vehicleId, DateOnly dateSold) : DomainEvent
{
    public Guid VehicleId { get; } = vehicleId;
    public DateOnly DateSold { get; } = dateSold;
}
