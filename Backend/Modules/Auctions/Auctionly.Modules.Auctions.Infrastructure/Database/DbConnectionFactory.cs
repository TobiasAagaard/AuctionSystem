
using System.Data.Common;
using Auctionly.Modules.Auctions.Application.Abstractions.Data;
using Npgsql;

namespace Auctionly.Modules.Auctions.Infrastructure.Database;

internal sealed class DbConnectionFactory(NpgsqlDataSource dataSource) : IDbConnectionFactory
{
    public async ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default) =>
        await dataSource.OpenConnectionAsync(cancellationToken);
}
