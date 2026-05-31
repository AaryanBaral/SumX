using MediatR;

namespace SumX.Application.Common.Abstractions.CQRS;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}
