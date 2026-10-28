namespace GarageLog.SharedKernel.Results;

/// <summary>
/// Результат операции
/// </summary>
public interface IResult
{
    /// <summary>
    /// Указывает, является ли результат успешными
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Статус результата
    /// </summary>
    ResultStatus ResultStatus { get; }

    /// <summary>
    /// Тип объекта, который лежит внутри результата
    /// </summary>
    Type ValueType { get; }

    /// <summary>
    /// Получает сам результат операции
    /// </summary>
    /// <returns></returns>
    object GetValue();

    /// <summary>
    /// Ошибки операции
    /// </summary>
    IEnumerable<string> Errors { get; }
}