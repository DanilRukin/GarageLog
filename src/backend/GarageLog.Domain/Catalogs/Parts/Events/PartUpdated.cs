using GarageLog.SharedKernel;

namespace GarageLog.Domain.Catalogs.Parts.Events;

/// <summary>Запчасть обновлена.</summary>
public class PartUpdated : DomainEvent
{
    public Guid Id { get; }
    public PartUpdated(Guid id) => Id = id;
}