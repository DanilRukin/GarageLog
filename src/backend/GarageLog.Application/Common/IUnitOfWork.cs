namespace GarageLog.Application.Common;

/// <summary>
/// Единица работы: оборачивает изменения агрегатов в транзакцию.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Сохраняет изменения и коммитит транзакцию.</summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
