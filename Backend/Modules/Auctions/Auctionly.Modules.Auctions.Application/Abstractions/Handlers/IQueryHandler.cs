
namespace Auctionly.Modules.Auctions.Application.Abstractions.Handlers;

public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken ct);
}