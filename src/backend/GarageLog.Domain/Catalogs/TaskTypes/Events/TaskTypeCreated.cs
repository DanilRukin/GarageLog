using GarageLog.SharedKernel;

namespace GarageLog.Domain.Catalogs.TaskTypes.Events;

/// <summary>Тип работы создан.</summary>
public class TaskTypeCreated : DomainEvent
{
    public Guid Id { get; }
    public string Name { get; }
    public TaskTypeCreated(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
