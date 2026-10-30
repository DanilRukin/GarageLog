namespace GarageLog.Domain.Planning;

/// <summary>
/// Канал напоминания
/// </summary>
public enum ReminderChannel
{
    /// <summary>
    /// В приложении
    /// </summary>
    InApp = 1,

    /// <summary>
    /// На почту
    /// </summary>
    Email = 2,

    /// <summary>
    /// Push уведомление
    /// </summary>
    Push = 3
}
