using System.Text;
using System.Text.Encodings.Web;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MyApplication.Server.Data;
using MyApplication.Server.Email.Interfaces;
using MyApplication.Shared.Results.Errors;

namespace MyApplication.Server.Features.Accounts.Commands;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly NavigationManager _navigationManager;
    private readonly IEmailService _emailService;
    
    public CreateAccountCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUserStore<ApplicationUser> userStore,
        NavigationManager navigationManager,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _navigationManager = navigationManager;
        _userStore = userStore;
        _emailService = emailService;
    }
    
    public async Task<Result> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _signInManager.UserManager.FindByEmailAsync(request.Email);
        if(user != null){
            return Result.Fail(new ValidationError(nameof(request.Email), "Email already exists"));
        }

        ApplicationUser userEntity = new ApplicationUser();
        user = new ApplicationUser();

        await _userStore.SetUserNameAsync(user, request.Email, CancellationToken.None);
        await ((IUserEmailStore<ApplicationUser>)_userStore).SetEmailAsync(user, request.Email, CancellationToken.None);

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            return Result.Fail(new CriticalError());
        }

        string userId = await _userManager.GetUserIdAsync(user);
        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        string callbackUrl = _navigationManager.GetUriWithQueryParameters(
            request.UrlForConfirmEmail,
            new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });

        await _emailService.SendConfirmationLinkForRegisteringAsync(request.Email, HtmlEncoder.Default.Encode(callbackUrl));

        return Result.Ok();
    }
}

public record CreateAccountCommand(string Email, string Password, string UrlForConfirmEmail) : IRequest<Result>;

