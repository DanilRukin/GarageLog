using GarageLog.Domain.Infrastructure;
using GarageLog.SharedKernel;
using static GarageLog.Domain.Errors.MoneyErrors;

namespace GarageLog.Domain.Common;

/// <summary>
/// Деньги
/// </summary>
public class Money : ValueObject
{
    /// <summary>
    /// Объем денежной величины
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Код валюты
    /// </summary>
    public string Currency { get; } = default!;
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    private Money(decimal amount,  string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Создает объект <see cref="Money"/> с 0 величиной
    /// </summary>
    public static Money Zero(string currrency) => new Money(0m, currrency);

    private static void EnsureSameCurrency(Money a, Money b)
    {
        if (!string.Equals(a.Currency, b.Currency, StringComparison.Ordinal))
            throw new DomainException<Money>(CurrencyMismatch);
    }

    /// <summary>
    /// Создает денежную величину нужного объема и нужной валюты
    /// </summary>
    public static Money Of(decimal amount, string currency)
    {
        if (amount < 0m)
            throw new DomainException<Money>(AmountMustBeNotNegative);
        return new Money(decimal.Round(amount, 2, MidpointRounding.AwayFromZero), NormilizeCurrency(currency));
    }

    public static Money operator +(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return Of(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return Of(a.Amount - b.Amount, a.Currency);
    }

    public static Money operator *(Money a, decimal qty)
    {
        if (qty < 0)
            throw new DomainException<Money>(NegativeMuliplier);
        return Of(a.Amount * qty, a.Currency);
    }

    private static string NormilizeCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException<Money>(CurrencyIsEmpty);
        string normilized = currency.Trim().ToUpperInvariant();
        if (normilized.Length != 3)
            throw new DomainException<Money>(CurrencyMustBeInISO);
        return normilized;
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";
}