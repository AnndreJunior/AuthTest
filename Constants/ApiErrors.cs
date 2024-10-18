using AuthTest.Results;

namespace AuthTest.Constants;

public static class ApiErrors
{
    public static readonly Error UnProcessableRequest = new(
        "Unprocessable Entity",
        "Request can not be processed.");

    public static readonly Error InternalServerError = new(
        "Internal Server Error",
        "Something went wrong.");
}
