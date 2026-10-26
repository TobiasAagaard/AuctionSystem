using Auctionly.Modules.Auctions.Application.Abstractions.Clock;

namespace Auctionly.Modules.Auctions.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
