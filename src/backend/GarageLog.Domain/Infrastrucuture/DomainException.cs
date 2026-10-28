using GarageLog.SharedKernel;

namespace GarageLog.Domain.Infrastrucuture;

/// <summary>
/// Исключение уровня домена
/// </summary>
public class DomainException<T> : Exception
{
    public DomainException(string errorCode) : base(ErrorCodeConstructor.Build<T>(errorCode)) { }

    public static string GetFullErrorCode(string error) => ErrorCodeConstructor.Build<T>(error);
}
