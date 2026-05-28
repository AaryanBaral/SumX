namespace SumX.API.Common;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public int StatusCode { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "Success")
        => new()
        {
            Success = true,
            Data = data,
            Message = message,
            StatusCode = 200
        };

    public static ApiResponse<T> Fail(string message, int statusCode)
        => new()
        {
            Success = false,
            Message = message,
            StatusCode = statusCode
        };
}