namespace Domain.Interfaces;

/// <summary>
/// Unit of Work pattern interface
/// Coordinates multiple repositories and manages transactions
/// </summary>
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Saves all changes made through repositories.
    /// All changes succeed or all fail (atomic transaction).
    /// </summary>
    /// <returns>Number of affected rows</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins an explicit transaction.
    /// Use when you need to call external services between database operations.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the transaction. Call after all operations succeed.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the transaction. Call when any operation fails.
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
