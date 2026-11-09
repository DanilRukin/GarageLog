using GarageLog.SharedKernel;

namespace GarageLog.Domain.Catalogs.Workshops.Events;

/// <summary>СТО создана.</summary>
public class WorkshopCreated : DomainEvent
{
    public Guid Id { get; }
    public string Name { get; }

    public WorkshopCreated(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
