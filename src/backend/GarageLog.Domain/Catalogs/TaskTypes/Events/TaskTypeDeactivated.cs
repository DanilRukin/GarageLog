using GarageLog.SharedKernel;

namespace GarageLog.Domain.Catalogs.TaskTypes.Events;

/// <summary>Тип работы деактивирован.</summary>
public class TaskTypeDeactivated : DomainEvent
{
    public Guid Id { get; }
    public TaskTypeDeactivated(Guid id) => Id = id;
}