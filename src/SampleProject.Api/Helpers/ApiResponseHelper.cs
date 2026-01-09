using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace SampleProject.Api.Helpers;

/// <summary>
/// 統一的 API 回應 Helper
/// 提供標準化的 API 回應格式，確保所有端點使用一致的結構
/// </summary>
public static class ApiResponseHelper
{
    /// <summary>
    /// 成功回應（200 OK）
    /// </summary>
    public static ActionResult<T> Ok<T>(T data, string? message = null)
    {
        var response = ApiResponse<T>.SuccessResponse(data, message);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// 成功回應（200 OK）- 無資料
    /// </summary>
    public static ActionResult Ok(string? message = null)
    {
        var response = ApiResponse.SuccessResponse(message);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// 建立回應（201 Created）
    /// </summary>
    public static ActionResult<T> Created<T>(T data, string actionName, object? routeValues, string? message = null)
    {
        var response = ApiResponse<T>.SuccessResponse(data, message);
        return new CreatedAtActionResult(actionName, null, routeValues, response);
    }

    /// <summary>
    /// 建立回應（201 Created）- 使用 URI
    /// </summary>
    public static ActionResult<T> Created<T>(string uri, T data, string? message = null)
    {
        var response = ApiResponse<T>.SuccessResponse(data, message);
        return new CreatedResult(uri, response);
    }

    /// <summary>
    /// 無內容回應（204 No Content）
    /// </summary>
    public static ActionResult NoContent(string? message = null)
    {
        return new NoContentResult();
    }

    /// <summary>
    /// 錯誤回應（400 Bad Request）
    /// </summary>
    public static ActionResult BadRequest(string message, string? code = null, string? description = null)
    {
        var errorCode = code ?? ErrorCodes.BadRequest.General;
        var response = ApiResponse.ErrorResponse(message, errorCode, description);
        return new BadRequestObjectResult(response);
    }

    /// <summary>
    /// 錯誤回應（400 Bad Request）- 使用 ErrorCodeInfo
    /// </summary>
    public static ActionResult BadRequest(string message, ErrorCodeInfo errorCodeInfo)
    {
        var response = ApiResponse.ErrorResponse(message, errorCodeInfo.Code, errorCodeInfo.Description);
        return new BadRequestObjectResult(response);
    }

    /// <summary>
    /// 驗證錯誤回應（400 Bad Request）
    /// </summary>
    public static ActionResult BadRequest(List<ValidationError> errors, string? message = "Validation failed", string? code = null)
    {
        var response = ApiResponse<object>.ValidationErrorResponse(errors, message, code ?? ErrorCodes.Validation.ValidationFailed);
        return new BadRequestObjectResult(response);
    }

    /// <summary>
    /// FluentValidation 驗證錯誤回應（400 Bad Request）
    /// </summary>
    public static ActionResult BadRequest(ValidationResult validationResult, string? message = "Validation failed")
    {
        var errors = validationResult.Errors.Select(e => new ValidationError
        {
            Field = e.PropertyName,
            Message = e.ErrorMessage,
            AttemptedValue = e.AttemptedValue
        }).ToList();

        return BadRequest(errors, message);
    }

    /// <summary>
    /// 未找到回應（404 Not Found）
    /// </summary>
    public static ActionResult NotFound(string message, string? code = null, string? description = null)
    {
        var errorCode = code ?? ErrorCodes.NotFound.General;
        var response = ApiResponse.ErrorResponse(message, errorCode, description);
        return new NotFoundObjectResult(response);
    }

    /// <summary>
    /// 未找到回應（404 Not Found）- 使用 ErrorCodeInfo
    /// </summary>
    public static ActionResult NotFound(string message, ErrorCodeInfo errorCodeInfo)
    {
        var response = ApiResponse.ErrorResponse(message, errorCodeInfo.Code, errorCodeInfo.Description);
        return new NotFoundObjectResult(response);
    }

    /// <summary>
    /// 未授權回應（401 Unauthorized）
    /// </summary>
    public static ActionResult Unauthorized(string message = "Unauthorized", string? code = null, string? description = null)
    {
        var errorCode = code ?? ErrorCodes.Unauthorized.General;
        var response = ApiResponse.ErrorResponse(message, errorCode, description);
        return new UnauthorizedObjectResult(response);
    }

    /// <summary>
    /// 禁止回應（403 Forbidden）
    /// </summary>
    public static ActionResult Forbidden(string message = "Forbidden", string? code = null, string? description = null)
    {
        var errorCode = code ?? ErrorCodes.Forbidden.General;
        var response = ApiResponse.ErrorResponse(message, errorCode, description);
        return new ObjectResult(response)
        {
            StatusCode = 403
        };
    }

    /// <summary>
    /// 衝突回應（409 Conflict）
    /// </summary>
    public static ActionResult Conflict(string message, string? code = null, string? description = null)
    {
        var errorCode = code ?? ErrorCodes.Conflict.General;
        var response = ApiResponse.ErrorResponse(message, errorCode, description);
        return new ConflictObjectResult(response);
    }

    /// <summary>
    /// 內部伺服器錯誤回應（500 Internal Server Error）
    /// </summary>
    public static ActionResult InternalServerError(string message = "An error occurred while processing your request", string? code = null, string? description = null)
    {
        var errorCode = code ?? ErrorCodes.InternalServer.General;
        var response = ApiResponse.ErrorResponse(message, errorCode, description);
        return new ObjectResult(response)
        {
            StatusCode = 500
        };
    }
}
