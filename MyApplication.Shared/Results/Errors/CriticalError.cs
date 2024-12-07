using FluentResults;

namespace MyApplication.Shared.Results.Errors;

public class CriticalError : Error
{
    public CriticalError()
    {
    }

    public CriticalError(string message) : base(message)
    {
    }
}
