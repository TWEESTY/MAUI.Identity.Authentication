using MyApplication.MAUI.Services.Interfaces;
using FluentResults;
using System.Net.Http.Json;
using MyApplication.Shared.Models.Accounts.Requests;
using MyApplication.Shared.Models.MobileAuthentication.Results;
using System.Net;
using MyApplication.Shared.Errors;
using MyApplication.Shared.Models.MobileAuthentication.Requests;
using System.Globalization;

namespace MyApplication.MAUI.Services;

public class AuthenticationService(HttpClient httpClient, IUserService userService) : IAuthenticationService
{
    private const string AccessTokenKeyForStorage = "access_token";
    private const string AccessTokenExpirationKeyForStorage = "access_token_expiration";
    private const string RefreshTokenKeyForStorage = "refresh_token";
    private const string SchemeKeyForStorage = "scheme";
    private const string CallbackScheme = "myapp";
    private string MobileAuthenticationSuffixUrl => $"api/mobile-authentication";
    private readonly HttpClient _httpClient = httpClient;
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Deletes the access token, refresh token, access token expiration and scheme from the secure storage.
    /// </summary>
    /// <returns></returns>
    public Result Logout(){
        try{
            SecureStorage.Remove(AccessTokenKeyForStorage);
            SecureStorage.Remove(RefreshTokenKeyForStorage);
            SecureStorage.Remove(AccessTokenExpirationKeyForStorage);
            SecureStorage.Remove(SchemeKeyForStorage);

            _userService.ClearAllInformation();

            return Result.Ok();
        }
        catch(Exception){
            return Result.Fail("Failed to logout");
        }
    }

    /// <summary>
    /// Gets the access token silently.
    /// </summary>
    /// <returns>Access token</returns>
    public async Task<Result<string>> GetAccessTokenSilentlyAsync(){
        if(await IsValidAccessTokenAsync()){
            return Result.Ok((await SecureStorage.GetAsync(AccessTokenKeyForStorage))!);
        }

        Result<string> resultRefreshTokens = await RefreshTokensAsync();
        if(resultRefreshTokens.IsFailed){
            return Result.Fail("Failed to refresh tokens");
        }

        return resultRefreshTokens.Value;
    }

    /// <summary>
    /// Refresh tokens via refresh token
    /// </summary>
    /// <returns>Access token</returns>
    public async Task<Result<string>> RefreshTokensAsync(){
        try{

            string? currentRefreshToken = await SecureStorage.GetAsync(RefreshTokenKeyForStorage);

            if(string.IsNullOrEmpty(currentRefreshToken)){
                return Result.Fail("No refresh token found");
            }

            HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(
                $"refresh",
                new RefreshTokensResult(currentRefreshToken)
            );

            if(!responseMessage.IsSuccessStatusCode){
                return Result.Fail("Failed to refresh tokens");
            }

            AuthenticateMobileRequestResult? authenticateMobileRequestResult = await responseMessage.Content.ReadFromJsonAsync<AuthenticateMobileRequestResult>();

            if(authenticateMobileRequestResult == null){
                return Result.Fail("Failed to refresh tokens");
            }

            await StoreTokensAsync(
                authenticateMobileRequestResult.AccessToken,
                authenticateMobileRequestResult.RefreshToken,
                DateTime.UtcNow.AddSeconds(authenticateMobileRequestResult.ExpiresIn)
            );

            return Result.Ok(authenticateMobileRequestResult.AccessToken);
        }
        catch(Exception){
            return Result.Fail("Failed to refresh tokens");
        }
    }


    /// <summary>
    /// Authenticates the user without external provider.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns>Access token</returns>
    public async Task<Result<string>> AuthenticateWithoutProviderAsync(string email, string password){
        try{
            HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(
                $"login?useCookies=false&useSessionCookies=false",
                new LoginRequest(email, password)
            );

            if(!responseMessage.IsSuccessStatusCode && responseMessage.StatusCode == HttpStatusCode.Unauthorized){
                return Result.Fail(new IncorrectCredentialsError());
            }

            if(!responseMessage.IsSuccessStatusCode){
                return Result.Fail(new UnknownError("Failed to authenticate via provider"));
            }

            AuthenticateMobileRequestResult? authenticateMobileRequestResult = await responseMessage.Content.ReadFromJsonAsync<AuthenticateMobileRequestResult>();

            if(authenticateMobileRequestResult == null){
                return Result.Fail(new UnknownError("Failed to authenticate via provider"));
            }

            await StoreTokensAsync(
                authenticateMobileRequestResult.AccessToken, 
                authenticateMobileRequestResult.RefreshToken, 
                DateTime.UtcNow.AddSeconds(authenticateMobileRequestResult.ExpiresIn)
            );

            return Result.Ok(authenticateMobileRequestResult.AccessToken);
        }
        catch (TaskCanceledException){
            return Result.Fail(new UnknownError("Task was canceled"));
        }
        catch (Exception){
            return Result.Fail(new UnknownError("Failed to authenticate via provider"));
        }
    }

    /// <summary>
    /// Authenticates the user via the provider.
    /// </summary>
    /// <param name="scheme">Name of the provider</param>
    /// <returns>Access token</returns>
    public async Task<Result<string>> AuthenticateViaProviderAsync(string scheme){
        try{
            
            WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
                new WebAuthenticatorOptions()
                {
                    Url = new Uri($"{_httpClient.BaseAddress}{MobileAuthenticationSuffixUrl}/{scheme}/identify"),
                    CallbackUrl = new Uri($"{CallbackScheme}://"),
                    PrefersEphemeralWebBrowserSession = true
                }
            );
                
            string accessToken = authResult.AccessToken;
            string email = authResult.Properties["email"];

            HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(
                $"{MobileAuthenticationSuffixUrl}/{scheme}/authenticate", 
                new AuthenticateMobileRequest (AccessToken: accessToken, EmailAddress: email)
            );

            if(!responseMessage.IsSuccessStatusCode){
                return Result.Fail(new UnknownError("Failed to authenticate via provider"));
            }

            AuthenticateMobileRequestResult? authenticateMobileRequestResult = await responseMessage.Content.ReadFromJsonAsync<AuthenticateMobileRequestResult>();

            if(authenticateMobileRequestResult == null){
                return Result.Fail(new UnknownError("Failed to authenticate via provider"));
            }

            await StoreTokensAsync(
                authenticateMobileRequestResult.AccessToken, 
                authenticateMobileRequestResult.RefreshToken, 
                DateTime.UtcNow.AddSeconds(authenticateMobileRequestResult.ExpiresIn),
                scheme
            );

            return Result.Ok(authenticateMobileRequestResult.AccessToken);
        }
        catch (TaskCanceledException){
            return Result.Fail(new UnknownError("Task was canceled"));
        }
        catch (Exception){
            return Result.Fail(new UnknownError("Failed to authenticate via provider"));
        }
    }

    private async Task StoreTokensAsync(string accessToken, string refreshToken, DateTime accessTokenExpiration, string? scheme = null){
        await SecureStorage.SetAsync(AccessTokenKeyForStorage, accessToken);
        await SecureStorage.SetAsync(RefreshTokenKeyForStorage, refreshToken);
        await SecureStorage.SetAsync(AccessTokenExpirationKeyForStorage, accessTokenExpiration.ToUniversalTime().ToString("G"));

        if(scheme != null){
            await SecureStorage.SetAsync(SchemeKeyForStorage, scheme);
        }
    }

    public async Task<bool> IsValidAccessTokenAsync(){
        string? accessTokenExpiration = await SecureStorage.GetAsync(AccessTokenExpirationKeyForStorage);

        if(string.IsNullOrEmpty(accessTokenExpiration)){
            return false;
        }

        if(!DateTime.TryParse(accessTokenExpiration,  CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime expirationDate)){
            return false;
        }

        return expirationDate > ( DateTime.UtcNow  + TimeSpan.FromMinutes(2));
    }


    public class IncorrectCredentialsError : Error
    {
    }
}
