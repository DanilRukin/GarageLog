using GarageLog.SharedKernel;

namespace GarageLog.Domain.Planning.Purchase.Events;

/// <summary>
/// План покупки создан.
/// </summary>
/// <param name="plannedPurchaseId">Идентификатор плана.</param>
/// <param name="vehicleId">Идентификатор автомобиля (если план привязан к машине).</param>
/// <param name="name">Наименование планируемой покупки.</param>
/// <param name="plannedForDate">Планируемая дата покупки (если задана).</param>
public class PlannedPurchaseCreated(
    Guid plannedPurchaseId,
    Guid? vehicleId,
    string name,
    DateTimeOffset? plannedForDate) : DomainEvent
{
    public Guid PlannedPurchaseId { get; } = plannedPurchaseId;
    public Guid? VehicleId { get; } = vehicleId;
    public string Name { get; } = name;
    public DateTimeOffset? PlannedForDate { get; } = plannedForDate;
}
