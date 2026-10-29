namespace GarageLog.Domain.Errors;

public static class MoneyErrors
{
    public const string CurrencyMismatch = "currency_mismatch";
    public const string CurrencyIsEmpty = "currency_is_empty";
    public const string CurrencyMustBeInISO = "currency_must_be_in_iso";
    public const string AmountMustBeNotNegative = "amount_must_be_not_negative";
}
