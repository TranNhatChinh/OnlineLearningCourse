namespace Application.Common.Wrappers;

/// <summary>
/// Standard API response wrapper for all endpoints
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public T? Data { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message, string? errorCode = null)
        => new() { Success = false, Message = message, ErrorCode = errorCode };

    public static ApiResponse<T> FailWithErrors(
        string message,
        IDictionary<string, string[]> errors,
        string? errorCode = null)
        => new() { Success = false, Message = message, ErrorCode = errorCode, Errors = errors };
}

/// Non-generic version for responses without data
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Ok(string? message = null)
        => new() { Success = true, Message = message };

    public static new ApiResponse Fail(string message, string? errorCode = null)
        => new() { Success = false, Message = message, ErrorCode = errorCode };

    public static new ApiResponse FailWithErrors(
        string message,
        IDictionary<string, string[]> errors,
        string? errorCode = null)
        => new() { Success = false, Message = message, ErrorCode = errorCode, Errors = errors };
}
