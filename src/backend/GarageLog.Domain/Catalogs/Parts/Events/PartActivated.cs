using GarageLog.SharedKernel;

namespace GarageLog.Domain.Catalogs.Parts.Events;

/// <summary>Запчасть активирована.</summary>
public class PartActivated : DomainEvent
{
    public Guid Id { get; }
    public PartActivated(Guid id) => Id = id;
}
