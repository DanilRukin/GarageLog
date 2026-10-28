namespace GarageLog.SharedKernel;

/// <summary>
/// Объект-значение
/// </summary>
public abstract class ValueObject
{
    public static bool operator ==(ValueObject left, ValueObject right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
        {
            return false;
        }
        return ReferenceEquals(left, null) || left.Equals(right);
    }

    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Получает набор компонентов объекта-значения,
    /// которые используются для сравнения с другими объектами-значениями
    /// </summary>
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// Получает копию этого объекта
    /// </summary>
    public ValueObject GetCopy()
    {
        return MemberwiseClone() as ValueObject;
    }
}