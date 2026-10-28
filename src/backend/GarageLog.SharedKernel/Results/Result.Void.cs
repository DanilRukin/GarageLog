namespace GarageLog.SharedKernel.Results;

/// <summary>
/// Результат операции
/// </summary>
public class Result : Result<Result>
{
    public Result() : base() { }
    protected internal Result(ResultStatus status) : base(status) { }

    /// <summary>
    /// Создает успешную операцию
    /// </summary>
    public static Result Success()
    {
        return new Result();
    }

    /// <summary>
    /// Создает успешную операцию
    /// </summary>
    /// <param name="value">Значение операции</param>
    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value);
    }

    /// <summary>
    /// Создает операцию с ошибкой
    /// </summary>
    /// <param name="errors">Ошибки</param>
    public static new Result Error(params string[] errorMessages)
    {
        return new Result(ResultStatus.Error) { Errors = errorMessages };
    }

    /// <summary>
    /// Создает операцию с типом "Не найдено"
    /// </summary>
    public static new Result NotFound()
    {
        return new Result(ResultStatus.NotFound);
    }

    /// <summary>
    /// Создает операцию с типом "Не найдено" и сообщениями об ошибках
    /// </summary>
    /// <param name="errorMessages">Ошибки</param>
    public static new Result NotFound(params string[] errorMessages)
    {
        return new Result(ResultStatus.NotFound) { Errors = errorMessages };
    }
}