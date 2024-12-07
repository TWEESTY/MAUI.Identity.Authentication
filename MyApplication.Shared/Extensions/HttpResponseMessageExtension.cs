using System.Net;
using System.Net.Http.Json;
using FluentResults;
using MyApplication.Shared.Results.Errors;

namespace MyApplication.Shared.Extensions;

public static class HttpResponseMessageExtension {

    public static async Task<Result<T>> ToResultWithContentAsync<T>(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            T? content = await response.Content.ReadFromJsonAsync<T>();
            return Result.Ok(content!);
        }
       
        return await GetErrorResultAsync(response);
    }

    public static async Task<Result> ToResultWithNoContentAsync(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return Result.Ok();
        }

        return await GetErrorResultAsync(response);
    }

    private static async Task<Result> GetErrorResultAsync(HttpResponseMessage response){
        List<ErrorContent>? errors = await response.Content.ReadFromJsonAsync<List<ErrorContent>>();
            
        switch (response.StatusCode)
        {
            case HttpStatusCode.NotFound:
                return Result.Fail(errors?.Select(e => e.Message).Select(e => new NotFoundError(e)));
            case HttpStatusCode.Unauthorized:
                return Result.Fail(errors?.Select(e => e.Message).Select(e => new UnauthorizedError(e)));
            case HttpStatusCode.Forbidden:
                return Result.Fail(errors?.Select(e => e.Message).Select(e => new ForbiddenError(e)));
            case HttpStatusCode.BadRequest:
                List<ValidationError> validationErrors = new();
                foreach (ErrorContent error in errors.AsNotNull())
                {
                    validationErrors.Add(new ValidationError(identifier: error.Identifier, error.Message));
                }
                return Result.Fail(validationErrors);
            default:
                return Result.Fail(errors?.Select(e => e.Message).Select(e => new CriticalError(e)));
        }
    } 

    /// <summary>
    ///  Error content returned by the API when there is/are error(s)
    /// </summary>
    public class ErrorContent{
        public string Identifier { get; set; } = null!;
        public string Message { get; set; } = null!;
    } 
}
