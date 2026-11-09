using GarageLog.SharedKernel;

namespace GarageLog.Domain.Catalogs.Parts.Events;

/// <summary>Запчасть создана.</summary>
public class PartCreated : DomainEvent
{
    public Guid Id { get; }
    public string Name { get; }
    public PartCreated(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
