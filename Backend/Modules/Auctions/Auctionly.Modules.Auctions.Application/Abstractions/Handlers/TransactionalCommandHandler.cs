using Auctionly.Modules.Auctions.Application.Abstractions.Data;
using Auctionly.Modules.Auctions.Domain.Abstractions;

namespace Auctionly.Modules.Auctions.Application.Abstractions.Handlers;

internal sealed class TransactionalCommandHandler<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> innerHandler,
    IUnitOfWork unitOfWork) : ICommandHandler<TCommand, TResult>
{
    public async Task<TResult> Handle(TCommand command, CancellationToken ct)
    {
        await unitOfWork.BeginAsync(ct);

        TResult result;
        try
        {
            result = await innerHandler.Handle(command, ct);
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }

        if (result is Result { IsFailure: true })
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            return result;
        }

        await unitOfWork.CommitAsync(ct);
        return result;
    }
}
