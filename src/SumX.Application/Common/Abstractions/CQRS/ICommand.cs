using MediatR;

namespace SumX.Application.Common.Abstractions.CQRS;

public interface ICommand<TResponse> : IRequest<TResponse>
{
}
