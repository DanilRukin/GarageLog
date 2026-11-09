using GarageLog.SharedKernel;

namespace GarageLog.Domain.Catalogs.Parts.Events;

/// <summary>Запчасть деактивирована.</summary>
public class PartDeactivated : DomainEvent
{
    public Guid Id { get; }
    public PartDeactivated(Guid id) => Id = id;
}
