using GarageLog.Application.Common;

namespace GarageLog.Infrastructure.Persistence;

/// <summary>Реализация <see cref="IUnitOfWork"/> поверх EF Core контекста.</summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly GarageLogDbContext _db;

    /// <summary>Создаёт unit of work.</summary>
    public UnitOfWork(GarageLogDbContext db) => _db = db;

    /// <inheritdoc/>
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}