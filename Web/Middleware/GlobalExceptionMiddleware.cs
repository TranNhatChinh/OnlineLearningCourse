using System.Net;
using Application.Common.Exceptions;
using Application.Common.Wrappers;
using Domain.Exceptions;

namespace Web.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var status = HttpStatusCode.InternalServerError;
        string message = "An unexpected error occurred. Please try again later.";
        string? errorCode = null;
        object? response = null;

        switch (exception)
        {
            case ApplicationValidationException ve:
                status = HttpStatusCode.BadRequest;
                message = ve.Message;
                errorCode = ve.ErrorCode;
                if (ve.Errors != null)
                    response = ApiResponse.FailWithErrors(message, ve.Errors, errorCode);
                break;


            case NotFoundException nf:
                status = HttpStatusCode.NotFound;
                message = nf.Message;
                errorCode = nf.ErrorCode;
                break;

            case ConflictException ce:
                status = HttpStatusCode.Conflict;
                message = ce.Message;
                errorCode = ce.ErrorCode;
                break;

            case ForbiddenException fe:
                status = HttpStatusCode.Forbidden;
                message = fe.Message;
                errorCode = fe.ErrorCode;
                break;

            case DomainException de:
                status = HttpStatusCode.BadRequest;
                message = de.Message;
                errorCode = de.ErrorCode;
                break;

            default:
                _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
                errorCode = "INTERNAL_SERVER_ERROR";
                break;
        }

        // Use pre-built response if available (for validation errors with Errors property)
        response ??= ApiResponse.Fail(message, errorCode);

        context.Response.StatusCode = (int)status;
        await context.Response.WriteAsJsonAsync(response);
    }
}
