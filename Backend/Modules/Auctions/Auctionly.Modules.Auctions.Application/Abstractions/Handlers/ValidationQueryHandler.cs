using FluentValidation;
using FluentValidation.Results;

namespace Auctionly.Modules.Auctions.Application.Abstractions.Handlers;

internal sealed class ValidationQueryHandler<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> innerHandler,
    IEnumerable<IValidator<TQuery>> validators) : IQueryHandler<TQuery, TResult>
{
    public async Task<TResult> Handle(TQuery query, CancellationToken ct)
    {
        var context = new ValidationContext<TQuery>(query);
        ValidationResult[] results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, ct)));

        var failures = results.SelectMany(r => r.Errors).ToList();

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }

        return await innerHandler.Handle(query, ct);
    }
}
