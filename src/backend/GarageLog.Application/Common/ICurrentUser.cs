namespace GarageLog.Application.Common;

/// <summary>
/// Текущий пользователь. В MVP — фиксированный owner.
/// </summary>
public interface ICurrentUser
{
    /// <summary>Идентификатор пользователя-владельца данных.</summary>
    Guid UserId { get; }
}
