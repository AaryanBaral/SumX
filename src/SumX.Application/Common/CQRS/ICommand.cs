using MediatR;

namespace SumX.Application.Common.CQRS;

public interface ICommand<TResponse> : IRequest<TResponse>
{
}
