using GarageLog.SharedKernel;

namespace GarageLog.Domain.Catalogs.TaskTypes.Events;

/// <summary>Тип работы активирован.</summary>
public class TaskTypeActivated : DomainEvent
{
    public Guid Id { get; }
    public TaskTypeActivated(Guid id) => Id = id;
}
