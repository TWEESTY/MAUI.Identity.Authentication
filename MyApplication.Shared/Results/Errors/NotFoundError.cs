using FluentResults;

namespace MyApplication.Shared.Results.Errors;

public class NotFoundError : Error
{
    public NotFoundError() : base()
    {
    }
    
    public NotFoundError(string message) : base(message)
    {
    }
}
