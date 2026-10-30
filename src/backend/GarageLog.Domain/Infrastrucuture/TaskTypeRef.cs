using static GarageLog.Domain.Errors.TaskTypeRefErrors;

namespace GarageLog.Domain.Infrastrucuture;

public sealed record TaskTypeRef(Guid TaskTypeId, string NameSnapshot)
{
    public static TaskTypeRef Create(Guid id, string name)
    {
        if (id == default(Guid))
            throw new DomainException<TaskTypeRef>(TaskTypeRefIdIsEmpty);
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException<TaskTypeRef>(TaskTypeRefNameIsEmpty);
        return new TaskTypeRef(id, name);
    }
}
