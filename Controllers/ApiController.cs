using AuthTest.Constants;
using AuthTest.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthTest.Controllers;

[Authorize]
[ApiController]
public abstract class ApiController : ControllerBase
{
    protected IActionResult HandleFailure(Result result) =>
        result switch
        {
            _ => StatusCode(
                StatusCodes.Status500InternalServerError,
                CreateProblemDetails(
                    ApiErrors.InternalServerError.Key,
                    result.StatusCode,
                    result.Error))
        };

    private static ProblemDetails CreateProblemDetails(
        string title,
        int statusCode,
        Error error) =>
        new()
        {
            Title = title,
            Type = error.Key,
            Detail = error.Message,
            Status = statusCode
        };
}
