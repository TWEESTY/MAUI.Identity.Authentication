using System.Net.Http.Json;
using FluentResults;
using MyApplication.Shared.Extensions;
using MyApplication.Shared.Models.Accounts.Requests;
using MyApplication.Shared.Models.Accounts.Results;
using MyApplication.Shared.Results.Errors;
using MyApplication.Shared.Services.Interfaces;

namespace MyApplication.Shared.Services;

public class ClientAccountProviderService : IAccountProviderService
{
    private readonly HttpClient _authenticatedHttpClient;
    private readonly HttpClient _unauthenticatedHttpClient;
    public ClientAccountProviderService(HttpClient authenticatedHttpClient, HttpClient unauthenticatedHttpClient)
    {
        _authenticatedHttpClient = authenticatedHttpClient;
        _unauthenticatedHttpClient = unauthenticatedHttpClient;
    }

    public async Task<Result> CreateAccountAsync(CreateAccountRequest request, CancellationToken cancellationToken = default)
    {
        try{
            using HttpResponseMessage responseMessage = await _authenticatedHttpClient.PostAsJsonAsync("api/accounts", request, cancellationToken);
            return await responseMessage.ToResultWithNoContentAsync();
        }
        catch{
            return Result.Fail(new CriticalError());
        }
    }

    public async Task<Result> DeleteAccountAsync(string idAccount, CancellationToken cancellationToken = default)
    {
        try{
            using HttpResponseMessage responseMessage = await _authenticatedHttpClient.DeleteAsync($"api/accounts/{idAccount}", cancellationToken);
            return await responseMessage.ToResultWithNoContentAsync();
        }
        catch{
            return Result.Fail(new CriticalError());
        }
    }

    public async Task<Result<GetClaimsAccountResult>> GetClaimsAccountAsync(CancellationToken cancellationToken = default)
    {
        try{
            using HttpResponseMessage responseMessage = await _authenticatedHttpClient.GetAsync($"api/accounts/claims", cancellationToken);
            return await responseMessage.ToResultWithContentAsync<GetClaimsAccountResult>();
        }
        catch{
            return Result.Fail(new CriticalError());
        }
    }

    public async Task<Result> ReactivateAccountAsync(string idAccount, CancellationToken cancellationToken = default)
    {
        try{
            using HttpResponseMessage responseMessage = await _authenticatedHttpClient.PostAsync($"api/accounts/{idAccount}/reactivate", null, cancellationToken);
            return await responseMessage.ToResultWithNoContentAsync();
        }
        catch{
            return Result.Fail(new CriticalError());;
        }
    }

    public async Task<Result> SoftDeleteAccountAsync(string idAccount, CancellationToken cancellationToken = default)
    {
        try{
            using HttpResponseMessage responseMessage = await _authenticatedHttpClient.PostAsync($"api/accounts/{idAccount}/soft-delete", null, cancellationToken);
            return await responseMessage.ToResultWithNoContentAsync();
        }
        catch{
            return Result.Fail(new CriticalError());;
        }
    }

    public async Task<Result> ForgotPasswordAccountAsync(ForgotPasswordAccountRequest request, CancellationToken cancellationToken = default)
    {
        try{
            using HttpResponseMessage responseMessage = await _unauthenticatedHttpClient.PostAsJsonAsync($"api/accounts/forgot-password", request, cancellationToken);
            return await responseMessage.ToResultWithNoContentAsync();
        }
        catch{
            return Result.Fail(new CriticalError());;
        }
    }

    public async Task<Result> ResetPasswordAccountAsync(ResetPasswordAccountRequest request, CancellationToken cancellationToken = default)
    {
        try{
            using HttpResponseMessage responseMessage = await _unauthenticatedHttpClient.PostAsJsonAsync($"api/accounts/reset-password", request, cancellationToken);
            return await responseMessage.ToResultWithNoContentAsync();
        }
        catch{
            return Result.Fail(new CriticalError());;
        }
    }

    public async Task<Result> ChangePasswordAccountAsync(ChangePasswordAccountRequest request, CancellationToken cancellationToken = default)
    {
        try{
            using HttpResponseMessage responseMessage = await _authenticatedHttpClient.PostAsJsonAsync($"api/accounts/change-password", request, cancellationToken);
            return await responseMessage.ToResultWithNoContentAsync();
        }
        catch{
            return Result.Fail(new CriticalError());;
        }
    }
}
