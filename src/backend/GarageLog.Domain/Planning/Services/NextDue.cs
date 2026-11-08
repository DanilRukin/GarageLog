namespace GarageLog.Domain.Planning.Services;

/// <summary>
/// Результат вычисления следующего срока: дата и/или пробег.
/// </summary>
/// <param name="DueDate">Срок по дате (если применимо).</param>
/// <param name="DueOdometer">Срок по пробегу (если применимо).</param>
public sealed record NextDue(DateTimeOffset? DueDate, int? DueOdometer)
{
    /// <summary>True, если срок вообще не определён (например, триггер пустой).</summary>
    public bool IsUndefined => DueDate is null && DueOdometer is null;
}
