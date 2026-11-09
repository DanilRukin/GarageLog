using GarageLog.SharedKernel;

namespace GarageLog.Domain.Catalogs.Workshops.Events;

/// <summary>СТО активирована.</summary>
public class WorkshopActivated : DomainEvent
{
    public Guid Id { get; }
    public WorkshopActivated(Guid id) => Id = id;
}
