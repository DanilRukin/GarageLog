using GarageLog.SharedKernel;

namespace GarageLog.Domain.Attachments.Events;

/// <summary>
/// Вложение создано (в статусе Pending).
/// </summary>
public class AttachmentCreated : DomainEvent
{
    public Guid AttachmentId { get; }
    public string FileName { get; }
    public long SizeBytes { get; }
    public AttachmentCreated(Guid attachmentId, string fileName, long sizeBytes)
    {
        AttachmentId = attachmentId;
        FileName = fileName;
        SizeBytes = sizeBytes;
    }
}
