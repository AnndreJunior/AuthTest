namespace AuthTest.Results;

public record Error(string Key, string Message)
{
    public static readonly Error None = new("", "");
}
