namespace UI.Controllers;

public class ErrorController : Controller
{
    [Route("error/{statusCode}")]
    public IActionResult HandleError(int statusCode, string? message = null)
    {
        ViewData["StatusCode"] = statusCode;
        ViewData["Message"] = message ?? statusCode switch
        {
            400 => "Bad Request. Please check your input.",
            401 => "Unauthorized. Please login to continue.",
            403 => "Forbidden. You don’t have permission.",
            404 => "Page not found.",
            500 => "An internal server error occurred.",
            _ => "An unexpected error occurred."
        };

        return View("Error");
    }

    [Route("error")]
    public IActionResult Error()
    {
        ViewData["StatusCode"] = 500;
        ViewData["Message"] = "An unexpected server error occurred.";
        return View("Error");
    }
}
