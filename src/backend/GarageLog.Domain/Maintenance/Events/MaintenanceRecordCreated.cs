using GarageLog.SharedKernel;

namespace GarageLog.Domain.Maintenance.Events;

public class MaintenanceRecordCreated(
    Guid recordId,
    Guid vehicleId,
    Guid taskTypeId,
    DateTimeOffset performedAt,
    int odometerKilometers
    ) : DomainEvent
{
    public Guid RecordId { get; } = recordId;
    public Guid VehicleId { get; } = vehicleId;
    public Guid TaskTypeId { get; } = taskTypeId;
    public DateTimeOffset PerformedAt { get; } = performedAt;
    public int OdometerKilometers { get; } = odometerKilometers;
}
