using FluentResults;

namespace MyApplication.Shared.Results.Errors;

public class UnauthorizedError : Error
{
    public UnauthorizedError() : base()
    {
    }

    public UnauthorizedError(string message) : base(message)
    {
    }
}
