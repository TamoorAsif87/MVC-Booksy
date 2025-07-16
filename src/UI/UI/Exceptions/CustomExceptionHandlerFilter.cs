using Microsoft.AspNetCore.Mvc.Filters;

namespace UI.Exceptions;

public class CustomExceptionHandlerFilter : IExceptionFilter
{
    private readonly ILogger<CustomExceptionHandlerFilter> _logger;

    public CustomExceptionHandlerFilter(ILogger<CustomExceptionHandlerFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        // Log the technical details
        _logger.LogError(exception, "Unhandled exception occurred");

        // Determine status code and message using switch expression
        (int statusCode, string userMessage) = exception switch
        {
            UnauthorizedAccessException => 
            (
                StatusCodes.Status401Unauthorized, 
                "You are not authorized to access this resource."
            ),
            ArgumentNullException or ArgumentException => 
            (   
                StatusCodes.Status400BadRequest, 
                "Invalid input provided."
            ),

            KeyNotFoundException => 
            (
                StatusCodes.Status404NotFound, 
                "Requested resource not found."
            ),
            ApplicationException => 
            (
                StatusCodes.Status400BadRequest, 
                exception.Message
            ),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.")
        };

        context.Result = new RedirectToActionResult("HandleError", "Error", new { statusCode, message = userMessage });

    }

}
