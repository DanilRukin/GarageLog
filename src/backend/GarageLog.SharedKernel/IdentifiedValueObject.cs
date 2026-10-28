namespace GarageLog.SharedKernel;

/// <summary>
/// Объект-значение с Id. Используется в случаях, когда просто необходимо хранить объект-значение как сущность БД
/// </summary>
/// <typeparam name="TKey">Тип данных ключа</typeparam>
public abstract class IdentifiedValueObject<TKey> : ValueObject
{
    protected TKey Id { get; set; }
}
