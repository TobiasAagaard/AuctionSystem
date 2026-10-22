using System.Data.Common;

namespace Auctionly.Modules.Auctions.Application.Abstractions.Data;

public interface IDbConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);

}