using System.Data.Common;
using Auctionly.Modules.Auctions.Application.Abstractions.Data;

namespace Auctionly.Modules.Auctions.Infrastructure.Database;

internal sealed class UnitOfWork(IDbConnectionFactory connectionFactory) : IUnitOfWork, IDbSession, IAsyncDisposable
{
    private DbConnection? _connection;
    private DbTransaction? _transaction;

    public DbConnection Connection =>
        _connection ?? throw new InvalidOperationException(
            "No active unit of work. Writes must run inside a command handler.");

    public DbTransaction Transaction =>
        _transaction ?? throw new InvalidOperationException(
            "No active unit of work. Writes must run inside a command handler.");

    public async Task BeginAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            throw new InvalidOperationException("A unit of work is already in progress.");
        }

        _connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        _transaction = await _connection.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await Transaction.CommitAsync(cancellationToken);
        await CleanupAsync();
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
        }

        await CleanupAsync();
    }

    public ValueTask DisposeAsync() => new(CleanupAsync());

    private async Task CleanupAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}
