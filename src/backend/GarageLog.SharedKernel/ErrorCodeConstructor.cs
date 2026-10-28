namespace GarageLog.SharedKernel;

public static class ErrorCodeConstructor
{
    public static string Build<T>(string errorPurposeCode)
    {
        string root = typeof(ErrorCodeConstructor).Assembly.GetName().FullName;
        if (string.IsNullOrWhiteSpace(errorPurposeCode))
            throw new ArgumentException(CreateCode(root, "error_purpose_code_is_null_or_whitespace"));
        string entityRootCode = typeof(T).Namespace ?? typeof(T).Assembly.GetName().FullName;
        return CreateCode(entityRootCode ?? root, errorPurposeCode);
    }

    public static string Build<T>(T entity, string errorPurposeCode) => Build<T>(errorPurposeCode);

    private static string CreateCode(string rootCode, string errorCode) => $"{rootCode}.{errorCode}";
}