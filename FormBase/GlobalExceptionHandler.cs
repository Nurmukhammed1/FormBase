using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace FormBase;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled error occured. RequestId: {RequestId}", httpContext.TraceIdentifier);

        var response = new ErrorResponse
        {
            RequestId = httpContext.TraceIdentifier,
            Message = "An error occurred while processing your request."
        };
        
        (int statusCode, string message) = exception switch
        {
            TemplateNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            UnauthorizedTemplateAccessException => (StatusCodes.Status403Forbidden, "Access denied."),
            ArgumentException => (StatusCodes.Status400BadRequest, _environment.IsDevelopment() ? exception.Message : "Invalid request."),
            DbUpdateException => (StatusCodes.Status500InternalServerError, "Database error occurred."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        httpContext.Response.StatusCode = statusCode;
        response.Message = message;

        if (_environment.IsDevelopment())
        {
            response.Details = exception.ToString();
        }

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}

public class TemplateNotFoundException : Exception
{
    public TemplateNotFoundException(int templateId) 
        : base($"Template with ID {templateId} was not found.") { }
}

public class UnauthorizedTemplateAccessException : Exception
{
    public UnauthorizedTemplateAccessException(int templateId, string userId) 
        : base($"User {userId} does not have access to template {templateId}.") { }
}

public class ErrorResponse
{
    public string RequestId { get; set; }
    public string Message { get; set; }
    public string Details { get; set; }
}
