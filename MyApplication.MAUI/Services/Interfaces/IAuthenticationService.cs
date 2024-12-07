using FluentResults;

namespace MyApplication.MAUI.Services.Interfaces;

public interface IAuthenticationService
{
    public Result Logout();
    public Task<Result<string>> GetAccessTokenSilentlyAsync();

    public Task<Result<string>> RefreshTokensAsync();

    public Task<Result<string>> AuthenticateWithoutProviderAsync(string email, string password);

    public Task<Result<string>> AuthenticateViaProviderAsync(string scheme);
}
