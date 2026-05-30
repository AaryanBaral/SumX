namespace SumX.API.Models.Auth
{
    public sealed record RegisterRequest(
        string Email,
        string Password,
        string Role);
}
