namespace GarageLog.Domain.Errors;

public static class AttachmentErrors
{
    public const string ContentHashRequired = "content_hash_required";
    public const string StorageKeyEmpty = "storage_key_empty";
    public const string FilenameEmpty = "storage_key_empty";
    public const string MimeTypeEmpty = "mime_type_empty";
    public const string MimeTypeNotAllowed = "mime_type_not_allowed";
    public const string FilenameTooLong = "file_name_too_long";
    public const string FilenameContainsPath = "file_name_contains_path";
    public const string InvalidFileName = "invalid_file_name";
    public const string ActivateOnlyFromPendingStatus = "activate_only_from_pending_status";
    public const string InvalidSize = "invalid_size";
    public const string SizeExceedsLimit = "size_exceeds_limit";
}
