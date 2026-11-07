namespace GarageLog.Domain.Errors;

public static class PlannedPurchaseErrors
{
    public const string OperationAvailableOnlyInPlannedStatus = "operation_available_only_in_planned_status";
    public const string QualityIsNotPositive = "quality_is_not_positive";
    public const string PlannedDateIsInPast = "planned_date_is_in_past";
    public const string PurchaseNameIsEmpty = "purchase_name_is_empty";
    public const string FulFillPurchaseOnlyInPlannedStatus = "fulfill_purchase_only_in_planned_status";
    public const string CreatedPurchaseIdIsRequired = "created_purchase_id_is_required";
    public const string CancellationReasonIsRequired = "cancellation_reason_is_required";
}
