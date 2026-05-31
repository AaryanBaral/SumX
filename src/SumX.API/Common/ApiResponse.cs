namespace SumX.API.Common;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public T? Data { get; init; }

    public int StatusCode { get; init; }

    public object? Errors { get; init; }

    public static ApiResponse<T> Fail(string message, int statusCode, object? errors = null) =>
        new()
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors
        };
}
