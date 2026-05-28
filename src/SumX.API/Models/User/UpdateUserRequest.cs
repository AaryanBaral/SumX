namespace SumX.API.Models.User;

public sealed class UpdateUserRequest
{
    public string Email { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;
}
