using FluentResults;

namespace MyApplication.Shared.Results.Errors;

public class ForbiddenError : Error
{
    public ForbiddenError() : base()
    {
    }
    
    public ForbiddenError(string message) : base(message)
    {
    }
}
