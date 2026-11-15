using System.Data.Common;

namespace Auctionly.Modules.Auctions.Infrastructure.Database;

internal interface IDbSession
{
    DbConnection Connection { get; }
    DbTransaction Transaction { get; }
}
