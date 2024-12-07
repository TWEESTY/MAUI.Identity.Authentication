using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyApplication.Shared.Models.MobileAuthentication.Requests;
using MyApplication.Server.Data;

namespace MyApplication.Server.Controllers;

/// <summary>
/// Controller for mobile authentication endpoints
/// </summary>
[ApiController]
public class MobileAuthenticationController(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IUserStore<ApplicationUser> userStore) : ControllerBase
{
    private const string CallbackScheme = "myapp";
    private const string ExternalAccessTokenKeyClaim = "external_access_token";
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUserStore<ApplicationUser> _userStore = userStore;


    [HttpGet("api/mobile-authentication/{scheme}/identify")]
    [EndpointDescription("Identify a user via a provider")]
    [EndpointSummary("Identifies a user via a provider")]
    [EndpointName("MobileAuthentication_Scheme_Identify")]
    [Tags("MobileAuthenticationEndpoint")]
    public async Task IdentifyViaProviderAsync([FromRoute] string scheme)
    {
        AuthenticateResult auth = await Request.HttpContext.AuthenticateAsync(scheme);

        if (!auth.Succeeded
            || auth?.Principal == null
            || !auth.Principal.Identities.Any(id => id.IsAuthenticated)
            || string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token")))
        {
            // Not authenticated, challenge
            await Request.HttpContext.ChallengeAsync(scheme);
            return;
        }

        IEnumerable<Claim> claims = auth.Principal.Identities.FirstOrDefault()!.Claims;
        string? email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        string? accessTokenFromExternalProvider = auth.Properties.GetTokenValue("access_token");

        if(string.IsNullOrEmpty(email)){
            throw new Exception("Email claim not found");
        }

        if(string.IsNullOrEmpty(accessTokenFromExternalProvider)){
            throw new Exception("Access token not found");
        }

        ApplicationUser? user = await _signInManager.UserManager.FindByEmailAsync(email);
        // Create account if user does not exist
        if(user == null){
            await CreateAccountAsync(email, new UserLoginInfo(scheme, email, scheme));
            user = await _signInManager.UserManager.FindByEmailAsync(email);
        }

        // Add the external access token claim in order to verify it later
        await _userManager.AddClaimAsync(user!, new Claim(ExternalAccessTokenKeyClaim, accessTokenFromExternalProvider));

        var redirectQueryString = new Dictionary<string, string>
        {
            { "access_token", accessTokenFromExternalProvider },
            { "email", email }
        };

        // Build the result url
        var url = CallbackScheme + "://#" + string.Join(
            "&",
            redirectQueryString.Where(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value != "-1")
            .Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

        // Redirect to final url
        Request.HttpContext.Response.Redirect(url);
    }

    [HttpPost("api/mobile-authentication/{scheme}/authenticate")]
    public async Task AuthenticateAsync([FromRoute] string scheme, [FromBody] AuthenticateMobileRequest request){
        ApplicationUser? user = await _signInManager.UserManager.FindByEmailAsync(request.EmailAddress);

        if(user == null){
            throw new Exception("User not found");
        }

        IList<Claim> claims = await _signInManager.UserManager.GetClaimsAsync(user);
        Claim? externalAccessTokenClaim = claims.FirstOrDefault(c => c.Type == ExternalAccessTokenKeyClaim);

        if(externalAccessTokenClaim == null || externalAccessTokenClaim.Value != request.AccessToken){
            throw new Exception("Invalid access token");
        }

        _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
        await _signInManager.SignInAsync(user, isPersistent: false);
    }

    private async Task CreateAccountAsync(string emailAddress, UserLoginInfo userLoginInfo)
    {
        ApplicationUser user = new ApplicationUser();
        user.EmailConfirmed = true;

        await _userStore.SetUserNameAsync(user, emailAddress, CancellationToken.None);
        await ((IUserEmailStore<ApplicationUser>)_userStore).SetEmailAsync(user, emailAddress, CancellationToken.None);

        IdentityResult result = await _userManager.CreateAsync(user);
        
        if (result.Succeeded)
        {
            result = await _userManager.AddLoginAsync(user, userLoginInfo);
            if (!result.Succeeded)
            {
                throw new Exception($"Unexpected error occurred while creating an external login for user with ID '{user.Id}'.");
            }
        }
    }
}
