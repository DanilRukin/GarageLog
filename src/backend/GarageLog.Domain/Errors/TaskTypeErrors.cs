namespace GarageLog.Domain.Errors;

public static class TaskTypeErrors
{
    public const string NameIsEmpty = "name_is_empty";
    public const string NameTooLong = "name_too_long";
    public const string NotActive = "not_active";
    public const string InvalidInterval = "invalid_interval";
}
