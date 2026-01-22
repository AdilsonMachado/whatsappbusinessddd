using System.Net;
using System.Text.Json;
using FluentValidation;

namespace WhatsAppBusiness.WebAPI.Middleware;

/// <summary>
/// Middleware for handling exceptions globally
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the ExceptionHandlingMiddleware
    /// </summary>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
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

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "Validation failed",
                    Errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList()
                }),
            
            ArgumentNullException => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request: required parameter is missing",
                    Errors = new List<string> { exception.Message }
                }),
            
            ArgumentException => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request",
                    Errors = new List<string> { exception.Message }
                }),
            
            KeyNotFoundException => (
                HttpStatusCode.NotFound,
                new ErrorResponse
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "Resource not found",
                    Errors = new List<string> { exception.Message }
                }),
            
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                new ErrorResponse
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Message = "Unauthorized",
                    Errors = new List<string> { exception.Message }
                }),
            
            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "An error occurred while processing your request",
                    Errors = new List<string> { exception.Message }
                })
        };

        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonResponse = JsonSerializer.Serialize(message, options);
        await context.Response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// Error response model
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// HTTP status code
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detailed error messages
    /// </summary>
    public List<string> Errors { get; set; } = new();
}
