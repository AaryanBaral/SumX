using MediatR;
using SumX.Application.Common.Abstractions.CQRS;
using SumX.Application.Users.DTOs;

namespace SumX.Application.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IQuery<UserDto>;
