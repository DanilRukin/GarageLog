using GarageLog.SharedKernel;

namespace GarageLog.Domain.Attachments.Events;

/// <summary>
/// Вложение удалено (soft delete).
/// </summary>
public class AttachmentDeleted : DomainEvent
{
    public Guid AttachmentId { get; }
    public string? Reason { get; }
    public AttachmentDeleted(Guid attachmentId, string? reason)
    {
        AttachmentId = attachmentId;
        Reason = reason;
    }
}
