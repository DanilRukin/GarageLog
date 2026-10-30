namespace GarageLog.Domain.Errors;

public static class VehicleErrors
{
    public const string OwnerIdIsDefault = "owner_id_is_default";
    public const string MakeIsEmpty = "make_is_empty";
    public const string ModelIsEmpty = "model_is_empty";
    public const string InvalidYear = "invalid_year";
    public const string VehicleIsNotActive = "vehivle_is_not_active";
    public const string CorrectionRequiresSource = "correction_requires_source";
    public const string CorrectionRequiresReason = "correction_requires_reason";
    public const string CorrectionOnlyForDecrease = "correction_only_for_decrease";
    public const string ReadingTakenAtCanNotBeEarlierThanCurrentTakenAt = "reading_taken_at_can_not_be_earlier_than_current_taken_at";
    public const string SellDateInTheFuture = "sell_date_in_the_future";
    public const string VehiscleIsAlreadyArchived = "vehicle_is_already_archived";
}
