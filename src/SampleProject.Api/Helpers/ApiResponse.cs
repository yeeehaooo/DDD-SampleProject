namespace SampleProject.Api.Helpers;

/// <summary>
/// 統一的 API 回應結構
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public ApiError? Error { get; set; }
    public List<ValidationError>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, string? code = null, string? description = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = new ApiError
            {
                Message = message,
                Code = code,
                Description = description ?? (code != null ? ErrorCodes.GetDescription(code) : null)
            }
        };
    }

    public static ApiResponse<T> ValidationErrorResponse(List<ValidationError> errors, string? message = "Validation failed", string? code = null)
    {
        var errorCode = code ?? ErrorCodes.Validation.ValidationFailed;
        return new ApiResponse<T>
        {
            Success = false,
            Error = new ApiError
            {
                Message = message,
                Code = errorCode,
                Description = ErrorCodes.GetDescription(errorCode)
            },
            Errors = errors
        };
    }
}

/// <summary>
/// API 錯誤資訊
/// </summary>
public class ApiError
{
    public string Message { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// 驗證錯誤資訊
/// </summary>
public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? AttemptedValue { get; set; }
}

/// <summary>
/// 無資料的回應（用於 204 No Content）
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public ApiError? Error { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse SuccessResponse(string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public static ApiResponse ErrorResponse(string message, string? code = null, string? description = null)
    {
        return new ApiResponse
        {
            Success = false,
            Error = new ApiError
            {
                Message = message,
                Code = code,
                Description = description ?? (code != null ? ErrorCodes.GetDescription(code) : null)
            }
        };
    }
}
