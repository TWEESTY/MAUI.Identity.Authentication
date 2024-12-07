using FluentResults;

namespace MyApplication.Shared.Results.Errors;

public class ValidationError : Error {
    public string Identifier { get; set; }

    public ValidationError(string identifier, string message) : base(message) {
        Identifier = identifier;
    }
}
