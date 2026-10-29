namespace GarageLog.Domain.Planning.Triggers;

/// <summary>
/// Триггер плановой работы
/// </summary>
public abstract record Trigger
{
    public abstract string Describe();
}
