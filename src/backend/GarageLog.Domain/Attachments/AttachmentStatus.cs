namespace GarageLog.Domain.Attachments;

/// <summary>
/// Статус вложения в жизненном цикле.
/// </summary>
public enum AttachmentStatus
{
    /// <summary>Загружается: метаданные созданы, байты ещё не подтверждены.</summary>
    Pending = 1,

    /// <summary>Активно: файл загружен, метаданные зафиксированы.</summary>
    Active = 2,

    /// <summary>Удалено: soft delete, байты могут быть удалены фоновым процессом.</summary>
    Deleted = 3
}