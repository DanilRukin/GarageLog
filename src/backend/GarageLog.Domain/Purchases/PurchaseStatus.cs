namespace GarageLog.Domain.Purchases;

/// <summary>
/// Статус покупки в жизненном цикле.
/// </summary>
public enum PurchaseStatus
{
    /// <summary>Черновик: покупка ещё формируется, данные могут меняться.</summary>
    Draft = 1,

    /// <summary>Подтверждена: покупка совершена, данные зафиксированы.</summary>
    Confirmed = 2,

    /// <summary>Возвращена: покупка отменена или товар возвращён продавцу.</summary>
    Returned = 3
}
