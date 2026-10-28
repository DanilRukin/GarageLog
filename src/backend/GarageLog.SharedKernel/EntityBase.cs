using GarageLog.SharedKernel.Interfaces;

namespace GarageLog.SharedKernel;

/// <summary>
/// Базовая сущность домена
/// </summary>
/// <typeparam name="TKey">Тип данных первичного ключа сущности</typeparam>
public abstract class EntityBase<TKey> : IDomainObject, IEquatable<EntityBase<TKey>>
{
    /// <summary>
    /// Id сущности
    /// </summary>
    public TKey Id { get; protected set; }
    private List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents = _domainEvents ?? new List<DomainEvent>();
        if (domainEvent != null && !_domainEvents.Contains(domainEvent))
            _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent) => _domainEvents?.Remove(domainEvent);

    public void ClearDomainEvents() => _domainEvents?.Clear();

    protected bool EqualTo(object? obj)
    {
        if (obj == null || !(obj is EntityBase<TKey>))
            return false;

        if (ReferenceEquals(obj, this))
            return true;

        if (obj.GetType() != GetType())
            return false;

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (EqualTo(obj))
        {
            EntityBase<TKey> other = (EntityBase<TKey>)obj;
            return other.Id.Equals(this.Id);
        }
        return false;
    }

    public bool Equals(EntityBase<TKey>? other)
    {
        if (EqualTo(other))
        {
            return other.Id.Equals(this.Id);
        }
        return false;
    }
}
