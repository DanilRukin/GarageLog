using GarageLog.SharedKernel;

namespace GarageLog.Domain.Catalogs.Workshops.Events;

/// <summary>СТО деактивирована.</summary>
public class WorkshopDeactivated : DomainEvent
{
    public Guid Id { get; }
    public WorkshopDeactivated(Guid id) => Id = id;
}
