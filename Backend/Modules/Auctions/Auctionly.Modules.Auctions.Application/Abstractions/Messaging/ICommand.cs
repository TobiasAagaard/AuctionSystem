using MediatR;

namespace Auctionly.Modules.Auctions.Application.Abstractions.Messaging;

public interface ICommand<out TResponse> : IRequest<TResponse>;
