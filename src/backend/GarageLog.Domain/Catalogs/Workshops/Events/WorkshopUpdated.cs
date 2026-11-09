using GarageLog.SharedKernel;

namespace GarageLog.Domain.Catalogs.Workshops.Events;

/// <summary>СТО обновлена.</summary>
public class WorkshopUpdated : DomainEvent
{
    public Guid Id { get; }
    public WorkshopUpdated(Guid id) => Id = id;
}
