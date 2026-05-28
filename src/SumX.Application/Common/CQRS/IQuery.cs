using MediatR;

namespace SumX.Application.Common.CQRS;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}
