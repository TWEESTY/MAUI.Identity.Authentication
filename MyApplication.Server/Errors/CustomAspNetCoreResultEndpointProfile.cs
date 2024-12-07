using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using MyApplication.Shared.Results.Errors;

namespace MyApplication.Server.Errors;

public class CustomAspNetCoreResultEndpointProfile : DefaultAspNetCoreResultEndpointProfile
{
    public override ActionResult TransformFailedResultToActionResult(FailedResultToActionResultTransformationContext context)
    {
        var result = context.Result;

        if (result.HasError<UnauthorizedError>(out var unauthorizedErrors))
        {
            return new UnauthorizedObjectResult(unauthorizedErrors.First().Message);
        }

        if (result.HasError<NotFoundError>(out var notFoundErrors))
        {
            return new NotFoundObjectResult(notFoundErrors.First().Message);
        }

        if (result.HasError<ValidationError>(out var validationErrors))
        {
            return new BadRequestObjectResult(TransformValidationErrors(validationErrors));
        }

        return new StatusCodeResult(500);
    }

    private IEnumerable<BadRequestErrorDto> TransformValidationErrors(IEnumerable<ValidationError> domainErrors)
    {
        return domainErrors.Select(e => new BadRequestErrorDto(e.Message, e.Identifier));
    }

    public class BadRequestErrorDto : ErrorDto
    {
        public string Identifier { get; }

        public BadRequestErrorDto(string message, string identifier)
        {
            Message = message;
            Identifier = identifier;
        }
    }
}
