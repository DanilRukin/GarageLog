using GarageLog.Domain.Common;
using GarageLog.Domain.Infrastructure;
using GarageLog.SharedKernel;
using static GarageLog.Domain.Errors.PurchaseLineErrors;

namespace GarageLog.Domain.Purchases;

/// <summary>
/// Позиция покупки: одна строка в чеке (наименование, количество, цена за единицу).
/// Является value object внутри агрегата <see cref="Purchase"/>.
/// </summary>
public sealed class PurchaseLine : ValueObject
{
    /// <summary>
    /// Ссылка на запчасть в справочнике. Null — позиция задана свободным текстом
    /// (например, «масло на долив» без привязки к номенклатуре).
    /// </summary>
    public Guid? PartId { get; }

    /// <summary>
    /// Наименование позиции на момент создания. Снапшот, чтобы история не зависела
    /// от переименований в справочнике запчастей.
    /// </summary>
    public string NameSnapshot { get; }

    /// <summary>
    /// Количество. Может быть дробным (например, 4.5 литра масла).
    /// </summary>
    public decimal Quantity { get; }

    /// <summary>
    /// Единица измерения количества.
    /// </summary>
    public PartUnit Unit { get; }

    /// <summary>
    /// Цена за единицу товара.
    /// </summary>
    public Money UnitPrice { get; }

    /// <summary>
    /// Стоимость позиции: <see cref="UnitPrice"/> × <see cref="Quantity"/>.
    /// Вычисляется на лету; в БД не хранится.
    /// </summary>
    public Money LineTotal => UnitPrice * Quantity;

    private PurchaseLine() { }

    /// <summary>
    /// Приватный конструктор: создание только через <see cref="Create"/>.
    /// </summary>
    private PurchaseLine(Guid? partId, string nameSnapshot, decimal quantity, PartUnit unit, Money unitPrice)
    {
        PartId = partId;
        NameSnapshot = nameSnapshot;
        Quantity = quantity;
        Unit = unit;
        UnitPrice = unitPrice;
    }
    public static PurchaseLine Create(Guid? partId,
        string name,
        decimal quantity,
        Money unitPrice,
        PartUnit unit = PartUnit.Piece)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException<PurchaseLine>(PurchaseLineNameIsEmpty);
        if (quantity <= 0)
            throw new DomainException<PurchaseLine>(PurchaseLineInvalidQuantity);
        return new PurchaseLine(
            partId == Guid.Empty ? null : partId,
            name.Trim(),
            decimal.Round(quantity, 3),
            unit,
            unitPrice);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PartId;
        yield return NameSnapshot;
    }
}
