using GarageLog.SharedKernel;

namespace GarageLog.Domain.Purchases.Events;

/// <summary>
/// Покупка создана (в статусе Draft).
/// </summary>
/// <param name="purchaseId">Идентификатор покупки.</param>
/// <param name="vehicleId">Идентификатор автомобиля (если покупка привязана к машине).</param>
/// <param name="totalAmount">Итоговая сумма на момент создания (может быть 0, если линии ещё не добавлены).</param>
/// <param name="currency">Валюта покупки.</param>
public class PurchaseRegistered(
    Guid purchaseId,
    Guid? vehicleId,
    decimal totalAmount,
    string currency) : DomainEvent
{
    public Guid PurchaseId { get; } = purchaseId;
    public Guid? VehicleId { get; } = vehicleId;
    public decimal TotalAmount { get; } = totalAmount;
    public string Currency { get; } = currency;
}
