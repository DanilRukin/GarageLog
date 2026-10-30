using static GarageLog.Domain.Errors.AttachmentRefErrors;

namespace GarageLog.Domain.Infrastrucuture;

public sealed record AttachmentRef(Guid AttachmentId, string? Caption)
{
    public static AttachmentRef Create(Guid attachmentId, string? caption = null)
    {
        if (attachmentId == default(Guid))
            throw new DomainException<AttachmentRef>(AttachmentRefIdIsDefault);
        if (caption is { Length: > 500 })
            throw new DomainException<AttachmentRef>(AttachmentRefCaptionTooLong);
        return new AttachmentRef(attachmentId, caption?.Trim());
    }
}
