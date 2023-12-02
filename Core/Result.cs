namespace Core;

public sealed class Result<T> where T : notnull
{
    private Result(bool isSuccess, T? value, string errorMessage)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }
    public string ErrorMessage { get; }
    public T? Value { get; }

    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, string.Empty);
    }

    public static Result<T> Fail(string message)
    {
        return new Result<T>(false, default, message);
    }
}
