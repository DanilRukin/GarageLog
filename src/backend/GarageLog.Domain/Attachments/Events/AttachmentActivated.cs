using GarageLog.SharedKernel;

namespace GarageLog.Domain.Attachments.Events;

/// <summary>
/// Вложение активировано (файл загружен, метаданные зафиксированы).
/// </summary>
public class AttachmentActivated : DomainEvent
{
    public Guid AttachmentId { get; }
    public string StorageKey { get; }
    public AttachmentActivated(Guid attachmentId, string storageKey)
    {
        AttachmentId = attachmentId;
        StorageKey = storageKey;
    }
}
