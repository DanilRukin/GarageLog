namespace GarageLog.SharedKernel.Results;

/// <summary>
/// Результат типа <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T">Тип результата</typeparam>
public class Result<T> : IResult
{
    protected Result() { }
    public Result(T value)
    {
        Value = value;
        if (Value != null)
        {
            ValueType = Value.GetType();
        }
    }
    public bool IsSuccess => ResultStatus == ResultStatus.Ok;

    public ResultStatus ResultStatus { get; protected set; } = ResultStatus.Ok;

    public Type ValueType { get; protected set; }

    public IEnumerable<string> Errors { get; protected set; } = new List<string>();

    public object GetValue()
    {
        return Value;
    }

    /// <summary>
    /// Сам результат
    /// </summary>
    public T Value { get; }

    public static implicit operator T(Result<T> result) => result.Value;
    public static implicit operator Result<T>(T value) => new Result<T>(value);

    /// <summary>
    /// Создает успешную операцию
    /// </summary>
    /// <param name="value">Значение операции</param>
    public static Result<T> Success(T value) => new Result<T>(value);

    protected Result(ResultStatus resultStatus)
    {
        ResultStatus = resultStatus;
    }

    /// <summary>
    /// Создает операцию с ошибкой
    /// </summary>
    /// <param name="errors">Ошибки</param>
    public static Result<T> Error(params string[] errors) => new Result<T>(ResultStatus.Error) { Errors = errors };

    /// <summary>
    /// Создает операцию с типом "Не найдено"
    /// </summary>
    public static Result<T> NotFound()
    {
        return new Result<T>(ResultStatus.NotFound);
    }

    /// <summary>
    /// Создает операцию с типом "Не найдено" и сообщениями об ошибках
    /// </summary>
    /// <param name="errorMessages">Ошибки</param>
    public static Result<T> NotFound(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.NotFound) { Errors = errorMessages };
    }
}