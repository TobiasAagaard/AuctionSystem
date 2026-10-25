namespace Auctionly.Modules.Auctions.Application.Abstractions.Handlers;

public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken ct);
}