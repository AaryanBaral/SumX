using MediatR;
using SumX.Application.Common.Abstractions.CQRS;
using SumX.Application.Users.DTOs;

namespace SumX.Application.Users.Queries.GetUsers;

public sealed record GetUsersQuery : IQuery<IReadOnlyList<UserDto>>;
