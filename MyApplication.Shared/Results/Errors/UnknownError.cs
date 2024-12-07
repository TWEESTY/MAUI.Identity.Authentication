using FluentResults;

namespace MyApplication.Shared.Results.Errors;

public class UnknownError : Error
{
    public UnknownError()
    {
    }

    public UnknownError(string message) : base(message)
    {
    }
}
