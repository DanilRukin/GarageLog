using GarageLog.SharedKernel;

namespace GarageLog.Domain.Maintenance.Events;

public class MaintenanceRecordStarted(Guid recordId) : DomainEvent
{
    public Guid RecordId { get; } = recordId;
}
