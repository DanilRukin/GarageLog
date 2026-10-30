using static GarageLog.Domain.Errors.WorkshopRefErrors;

namespace GarageLog.Domain.Infrastrucuture;

public sealed record WorkshopRef(Guid? WorkshopId, string NameSnapshot)
{
    public static WorkshopRef Create(Guid? id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException<WorkshopRef>(WorkshopRefNameIsEmpty);
        return new WorkshopRef(id, name.Trim());
    }
}
