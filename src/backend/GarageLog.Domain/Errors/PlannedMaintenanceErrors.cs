namespace GarageLog.Domain.Errors;

public static class PlannedMaintenanceErrors
{
    public const string VehicleIdIsRequired = "vehicle_id_is_required";
    public const string TaskTypeRequired = "task_type_required";
    public const string TriggerRequired = "trigger_required";
    public const string ReminderRequired = "reminder_required";
    public const string ReminderIdRequired = "reminder_id_required";
    public const string ReminderNotFound = "reminder_not_found";
    public const string InvalidOdometer = "invalid_odometer";
    public const string StartOnlyFromPlannedOrSnoozedStatus = "start_only_from_planned_or_snoozed_status";
    public const string OperationAvailableOnlyForPlannedOrSnoozedStatus = "operation_available_only_for_planned_or_snoozed_status";
    public const string SnoozeOnlyFromPlannedOrSnoozedStatus = "snooze_only_from_planned_or_snoozed_status";
    public const string SnoozeInPast = "snooze_in_past";
    public const string NotSnoozed = "not_snoozed";
    public const string CancellationReasonRequired = "cancellation_reason_required";
    public const string MaintenanceRecordIdIsRequired = "maintenance_record_id_is_required";
    public const string IntervalRequiresLastDate = "interval_requires_last_date";
    public const string IntervalRequiresLastOdometer = "interval_requires_last_odometer";
    public const string CompleteOnlyFromPlannedInprogressOrSnoozedStatus = "snooze_only_from_planned_inprogress_or_snoozed_status";
}
