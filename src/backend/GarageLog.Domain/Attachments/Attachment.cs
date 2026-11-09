using GarageLog.Domain.Attachments.Events;
using GarageLog.Domain.Infrastructure;
using GarageLog.SharedKernel;
using GarageLog.SharedKernel.Interfaces;
using static GarageLog.Domain.Errors.AttachmentErrors;

namespace GarageLog.Domain.Attachments;

/// <summary>
/// Вложение (файл): фото, чек, скан, PDF. Хранит только метаданные;
/// содержимое лежит в blob storage по <see cref="StorageKey"/>.
/// Не знает о владельцах — связь хранится в агрегатах-владельцах через <see cref="AttachmentRef"/>.
/// </summary>
public class Attachment : EntityBase<Guid>, IAgregateRoot
{
    /// <summary>Максимальный размер файла по умолчанию (20 МБ).</summary>
    public const long DefaultMaxSizeBytes = 20L * 1024 * 1024;

    /// <summary>Допустимые MIME-типы по умолчанию.</summary>
    public static readonly IReadOnlySet<string> DefaultAllowedMimeTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/heic",
        "application/pdf"
    };

    /// <summary>Оригинальное имя файла (без пути, ≤ 256 символов).</summary>
    public string FileName { get; private set; } = default!;

    /// <summary>MIME-тип (например, "image/jpeg", "application/pdf").</summary>
    public string MimeType { get; private set; } = default!;

    /// <summary>Размер файла в байтах.</summary>
    public long SizeBytes { get; private set; }

    /// <summary>Ключ в blob storage. Устанавливается при активации.</summary>
    public string? StorageKey { get; private set; }

    /// <summary>SHA-256 хеш содержимого. Используется для дедупликации.</summary>
    public ContentHash ContentHash { get; private set; } = default!;

    /// <summary>Статус вложения.</summary>
    public AttachmentStatus Status { get; private set; }

    /// <summary>Момент создания (UTC).</summary>
    public DateTimeOffset CreatedAt { get; private init; }

    /// <summary>Момент последнего изменения (UTC).</summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>Момент активации (UTC). Null, если файл ещё не загружен.</summary>
    public DateTimeOffset? ActivatedAt { get; private set; }

    /// <summary>Момент удаления (UTC). Null, если не удалено.</summary>
    public DateTimeOffset? DeletedAt { get; private set; }

    /// <summary>Причина удаления (опционально).</summary>
    public string? DeletedReason { get; private set; }

    /// <summary>Конструктор для EF Core.</summary>
    private Attachment() { }

    /// <summary>
    /// Создаёт новое вложение в статусе <see cref="AttachmentStatus.Pending"/>.
    /// Байты ещё не загружены; storage key появится при <see cref="Activate"/>.
    /// </summary>
    /// <param name="fileName">Оригинальное имя файла (без пути).</param>
    /// <param name="mimeType">MIME-тип.</param>
    /// <param name="sizeBytes">Размер в байтах (&gt; 0).</param>
    /// <param name="contentHash">SHA-256 хеш содержимого.</param>
    /// <param name="maxSizeBytes">Максимально допустимый размер; по умолчанию <see cref="DefaultMaxSizeBytes"/>.</param>
    /// <param name="allowedMimeTypes">Допустимые MIME-типы; по умолчанию <see cref="DefaultAllowedMimeTypes"/>.</param>
    /// <exception cref="DomainException">Если параметры не проходят валидацию.</exception>
    public static Attachment Create(
        string fileName,
        string mimeType,
        long sizeBytes,
        ContentHash contentHash,
        long maxSizeBytes = DefaultMaxSizeBytes,
        IReadOnlySet<string>? allowedMimeTypes = null)
    {
        ValidateFileName(fileName);
        ValidateMimeType(mimeType, allowedMimeTypes ?? DefaultAllowedMimeTypes);
        ValidateSize(sizeBytes, maxSizeBytes);

        if (contentHash is null)
            throw new DomainException<Attachment>(ContentHashRequired);

        var now = DateTimeOffset.UtcNow;
        var attachment = new Attachment
        {
            Id = Guid.NewGuid(),
            FileName = fileName.Trim(),
            MimeType = mimeType.Trim().ToLowerInvariant(),
            SizeBytes = sizeBytes,
            ContentHash = contentHash,
            Status = AttachmentStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now
        };

        attachment.AddDomainEvent(new AttachmentCreated(
            attachment.Id, attachment.FileName, attachment.SizeBytes));

        return attachment;
    }

    /// <summary>
    /// Активирует вложение: файл успешно загружен в blob storage.
    /// Метаданные после активации менять нельзя.
    /// </summary>
    /// <param name="storageKey">Ключ в blob storage (непустой).</param>
    /// <exception cref="DomainException">Если статус не Pending или ключ пустой.</exception>
    public void Activate(string storageKey)
    {
        if (Status != AttachmentStatus.Pending)
            throw new DomainException<Attachment>(ActivateOnlyFromPendingStatus);

        if (string.IsNullOrWhiteSpace(storageKey))
            throw new DomainException<Attachment>(StorageKeyEmpty);

        StorageKey = storageKey.Trim();
        Status = AttachmentStatus.Active;
        ActivatedAt = DateTimeOffset.UtcNow;
        Touch();

        AddDomainEvent(new AttachmentActivated(Id, StorageKey));
    }

    /// <summary>
    /// Мягко удаляет вложение. Байты в blob storage могут быть удалены фоновым процессом.
    /// </summary>
    /// <param name="reason">Причина удаления (опционально).</param>
    public void Delete(string? reason = null)
    {
        if (Status == AttachmentStatus.Deleted)
            return; // идемпотентно

        Status = AttachmentStatus.Deleted;
        DeletedAt = DateTimeOffset.UtcNow;
        DeletedReason = reason?.Trim();
        Touch();

        AddDomainEvent(new AttachmentDeleted(Id, DeletedReason));
    }

    // ---------- Валидация ----------

    /// <summary>
    /// Проверяет имя файла: непустое, ≤ 256 символов, без path-разделителей,
    /// не является "." или "..".
    /// </summary>
    private static void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException<Attachment>(FilenameEmpty);
        var trimmed = fileName.Trim();

        if (trimmed.Length > 256)
            throw new DomainException<Attachment>(FilenameTooLong);

        if (trimmed is "." or "..")
            throw new DomainException<Attachment>(InvalidFileName);

        if (trimmed.IndexOfAny(new[] { '/', '\\', '\0' }) >= 0)
            throw new DomainException<Attachment>(FilenameContainsPath);
    }

    /// <summary>
    /// Проверяет MIME-тип: непустой и входит в разрешённый список.
    /// </summary>
    private static void ValidateMimeType(string mimeType, IReadOnlySet<string> allowed)
    {
        if (string.IsNullOrWhiteSpace(mimeType))
            throw new DomainException<Attachment>(MimeTypeEmpty);
        var normalized = mimeType.Trim().ToLowerInvariant();

        if (!allowed.Contains(normalized))
            throw new DomainException<Attachment>(MimeTypeNotAllowed);
    }

    /// <summary>
    /// Проверяет размер файла: &gt; 0 и ≤ лимита.
    /// </summary>
    private static void ValidateSize(long sizeBytes, long maxSizeBytes)
    {
        if (sizeBytes <= 0)
            throw new DomainException<Attachment>(InvalidSize);

        if (sizeBytes > maxSizeBytes)
            throw new DomainException<Attachment>(SizeExceedsLimit);
    }

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}
