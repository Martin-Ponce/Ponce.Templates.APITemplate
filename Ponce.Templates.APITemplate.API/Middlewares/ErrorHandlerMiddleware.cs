using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Ponce.Templates.APITemplate.API.Dtos.BaseResponses;
using Ponce.Templates.APITemplate.Domain.Exceptions;

namespace Ponce.Templates.APITemplate.API.Middlewares;

/// <summary>Turns exceptions into a status code and a consistent JSON error.</summary>
public class ErrorHandlerMiddleware(ILogger<ErrorHandlerMiddleware> logger, RequestDelegate next)
{
    private readonly ILogger<ErrorHandlerMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("response has already started"))
        {
            _logger.LogDebug(ex, "Response has already started, ignoring exception: {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var error = new ErrorObjectResponse();
        switch (exception)
        {
            case InvalidOperationAppException ex:
                _logger.LogWarning(ex, "Operation not allowed [Code: {Code}]: {Message}.", ex.Code, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case ValidationException ex:
                _logger.LogWarning(ex, "Argument error [Code: {Code}]: {Message}.", ex.Code, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case NotFoundException ex:
                _logger.LogWarning(ex, "Resource not found [Code: {Code}]: {Message}.", ex.Code, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            case UnauthorizedAppException ex when ex.Code == UnauthorizedAppException.INSUFFICIENT_PERMISSIONS:
                _logger.LogWarning(ex, "Access forbidden [Code: {Code}]: {Message}.", ex.Code, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                break;
            case UnauthorizedAppException ex:
                _logger.LogWarning(ex, "Access denied [Code: {Code}]: {Message}.", ex.Code, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;
            case BadHttpRequestException ex:
                _logger.LogWarning(ex, "Argument error: {Message}.", ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case ConflictException ex:
                _logger.LogWarning(ex, "Data conflict [Code: {Code}]: {Message}.", ex.Code, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                break;
            default:
                _logger.LogError(exception, "Unhandled error: {Message}.", exception.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }
        if (exception is AppException appException)
        {
            error.Code = appException.Code.ToString();
            error.Data = appException.Args?.Values.ToList() ?? [];
            error.Message = appException.Message;
        }
        else
        {
            error.Code = exception is BadHttpRequestException ? ValidationException.INVALID_FORMAT.ToString() : "InternalError";
            error.Message = exception is BadHttpRequestException ? exception.Message : "An unexpected error occurred.";
        }
        var response = new ExceptionResponse()
        {
            StatusCode = context.Response.StatusCode,
            Error = error,
            IsError = true
        };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonSerializerOptions));
    }
}
