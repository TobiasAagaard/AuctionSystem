using Auctionly.Modules.Auctions.Domain.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Auctionly.Modules.Auctions.Presentation.ApiResults;

public static class ApiResults
{
    public static IResult Problem(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Can't create a problem response from a successful result.");
        }

        return Results.Problem(
            title: GetTitle(result.Error),
            detail: GetDetail(result.Error),
            type: GetType(result.Error.Type),
            statusCode: GetStatusCode(result.Error.Type));
    }

    private static string GetTitle(Error error) =>
        error.Type == ErrorType.Failure ? "Server failure" : error.Code;

    private static string GetDetail(Error error) =>
        error.Type == ErrorType.Failure ? "An unexpected error occurred" : error.Description;

    private static string GetType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation or ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

    private static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation or ErrorType.Problem => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
}
