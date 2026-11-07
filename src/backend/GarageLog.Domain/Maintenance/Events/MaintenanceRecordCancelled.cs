using GarageLog.SharedKernel;

namespace GarageLog.Domain.Maintenance.Events;

public class MaintenanceRecordCancelled(
    Guid recordId,
    string reason) : DomainEvent
{
    public Guid RecordId { get; } = recordId;
    public string Reason { get; } = reason;
}
