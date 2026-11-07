namespace GarageLog.Domain.Errors;

public static class MaintenanceRecordErrors
{
    public const string RecordsVehicleIdCanNotBeDefault = "records_vehicle_id_can_not_be_default";
    public const string LaborCostIsNull = "labor_cost_is_null";
    public const string InvalidStatusTransition = "invalid_status_transition";
    public const string RecordIsNotEditable = "record_is_not_editable";
    public const string RecordInTerminalStatus = "record_in_terminal_status";
    public const string RecordFutureDate = "record_future_date";
    public const string InvalidInitialStatus = "invalid_initial_status";
    public const string LaborCostIsRequired = "labor_cost_is_required";
    public const string PartIsRequired = "part_is_required";
    public const string PartIsAlreadyAdded = "part_is_already_added";
    public const string PartNotFound = "part_not_found";
    public const string DuplicateAttachment = "duplicate_attachment";
    public const string AttachmentNotFound = "attachment_not_found";
    public const string PurchaseIdCanNotBeDefault = "purchase_id_can_not_be_default";
    public const string CancellationReasonIsRequired = "cancellation_reason_is_required";
}
