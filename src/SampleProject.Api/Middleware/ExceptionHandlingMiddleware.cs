using System.Net;
using System.Text.Json;
using FluentValidation;
using SampleProject.Api.Helpers;
using SampleProject.Domain.Exceptions;

namespace SampleProject.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, code, validationErrors) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                "Validation failed",
                ErrorCodes.Validation.ValidationFailed,
                validationEx.Errors.Select(e => new ValidationError
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage,
                    AttemptedValue = e.AttemptedValue
                }).ToList()),
            ArgumentNullException => (HttpStatusCode.BadRequest, exception.Message, ErrorCodes.Argument.ArgumentNull, null),
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message, ErrorCodes.Argument.ArgumentInvalid, null),
            DomainException => (HttpStatusCode.BadRequest, exception.Message, ErrorCodes.Domain.DomainError, null),
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message, ErrorCodes.NotFound.General, null),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message, ErrorCodes.Operation.InvalidOperation, null),
            _ => (HttpStatusCode.InternalServerError, "An error occurred while processing your request.", ErrorCodes.InternalServer.General, null)
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        object response;

        // 如果是驗證錯誤，使用 ValidationErrorResponse
        if (validationErrors != null && validationErrors.Any())
        {
            response = ApiResponse<object>.ValidationErrorResponse(
                validationErrors,
                message,
                code);
        }
        else
        {
            // 使用統一的 API 回應格式，自動包含描述
            var description = ErrorCodes.GetDescription(code);
            response = ApiResponse.ErrorResponse(message, code, description);
        }

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(json);
    }
}
