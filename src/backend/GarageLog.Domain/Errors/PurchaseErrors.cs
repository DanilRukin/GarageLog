namespace GarageLog.Domain.Errors;

public static class PurchaseErrors
{
    public const string VendorCanNotBeEmpty = "vendor_can_not_be_empty";
    public const string CurrencyCanNotBeEmpty = "currency_can_not_be_empty";
    public const string PurchaseCanNotBeInTheFuture = "purchase_can_not_be_in_the_future";
    public const string PurchaseLineIsRequired = "purchase_line_is_required";
    public const string PurchaseLineDuplicated = "purchase_line_duplicated";
    public const string PurchaseNameSnapshotIsEmpty = "purchase_name_snapshot_is_empty";
    public const string PurchaseLineNotFound = "purchase_line_not_found";
    public const string ReceiptIsRequired = "receipt_is_required";
    public const string InvalidTransition = "invalid_transition";
    public const string PurchaseLineIsEmpty = "purchase_line_is_empty";
    public const string PurchaseReturningReasonIsRequired = "purchase_returning_reason_is_required";
    public const string PurchaseAlreadyReturned = "purchase_already_returned";
    public const string PurchaseCurrencyMismatch = "purchase_currency_mismatch";
    public const string PurchaseIsNotEditable = "purchase_is_not_editable";
}
