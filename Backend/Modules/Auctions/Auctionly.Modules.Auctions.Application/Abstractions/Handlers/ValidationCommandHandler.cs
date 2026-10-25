using FluentValidation;
using FluentValidation.Results;

namespace Auctionly.Modules.Auctions.Application.Abstractions.Handlers;

internal sealed class ValidationCommandHandler<TCommand, TResult> (
    ICommandHandler<TCommand, TResult> innerHandler,
    IEnumerable<IValidator<TCommand>> validators) : ICommandHandler<TCommand, TResult>
{
    public async Task<TResult> Handle(TCommand command, CancellationToken ct)
    {
        var context = new ValidationContext<TCommand>(command);
        ValidationResult[] results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, ct)));

        var failures = results.SelectMany(r => r.Errors).ToList();

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }
        return await innerHandler.Handle(command, ct);
    }
    
}