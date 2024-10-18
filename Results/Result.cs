namespace AuthTest.Results;

public class Result<TData> : Result
{
    private readonly TData? _data;

    protected internal Result(
        TData? data,
        bool isSuccess,
        int statusCode,
        Error? error = null)
        : base(isSuccess, statusCode, error) =>
        _data = data;

    public TData Data => IsSuccess
        ? _data!
        : throw new InvalidOperationException(
            "Can not get the success data in a result failure.");

    public static implicit operator Result<TData>(TData data) => Success(data);
}

public class Result
{
    protected internal Result(bool isSuccess, int statusCode, Error? error = null)
    {
        if (isSuccess && error is not null)
        {
            throw new InvalidOperationException();
        }
        if (!isSuccess && error is null)
        {
            throw new InvalidOperationException();
        }
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Error = error ?? Error.None;
    }

    public bool IsSuccess { get; set; }
    public Error Error { get; set; }
    public int StatusCode { get; set; }
    public bool IsFailure => !IsSuccess;

    public static Result<T> Create<T>(T data, Error error) =>
        data is null
            ? Success(data)
            : Failure<T>(error, StatusCodes.Status422UnprocessableEntity);

    public static Result Success(int statusCode = StatusCodes.Status204NoContent) =>
        new(true, statusCode);

    public static Result<T> Success<T>(T data, int statusCode = StatusCodes.Status200OK) =>
        new(data, true, statusCode);

    public static Result Failure(Error error, int statusCode) =>
        new(false, statusCode, error);

    public static Result<T> Failure<T>(Error error, int statusCode) =>
        new(default, false, statusCode, error);
}
