using GarageLog.SharedKernel;

namespace GarageLog.Domain.Catalogs.TaskTypes.Events;

/// <summary>Тип работы обновлён.</summary>
public class TaskTypeUpdated : DomainEvent
{
    public Guid Id { get; }
    public TaskTypeUpdated(Guid id) => Id = id;
}
