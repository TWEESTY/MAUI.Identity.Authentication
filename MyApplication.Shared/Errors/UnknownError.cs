using FluentResults;

namespace MyApplication.Shared.Errors;

public class UnknownError : Error
{
    public UnknownError(string message) : base(message)
    {
    }
}