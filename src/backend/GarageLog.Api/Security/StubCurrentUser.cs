using GarageLog.Application.Common;

namespace GarageLog.Api.Security;

/// <summary>Временная реализация: всегда один и тот же owner (для MVP без auth).</summary>
public sealed class StubCurrentUser : ICurrentUser
{
    /// <summary>Фиксированный ownerId. Заменится после внедрения аутентификации.</summary>
    public static readonly Guid FixedOwnerId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <inheritdoc/>
    public Guid UserId => FixedOwnerId;
}
