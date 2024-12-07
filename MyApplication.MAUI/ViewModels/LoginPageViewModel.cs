using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MyApplication.MAUI.Models;
using MyApplication.MAUI.Pages;
using MyApplication.MAUI.Services;
using MyApplication.MAUI.Services.Interfaces;

namespace MyApplication.MAUI.ViewModels;

public partial class LoginPageViewModel(
    IAuthenticationService authenticationService,
    IAccountService accountService,
    IUserService userService) : ObservableObject
{
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IAccountService _accountService = accountService;
    private readonly IUserService _userService = userService;
    
    [ObservableProperty]
    private string? email;
    
    [ObservableProperty]
    private string? _password;

    public async Task InitializeAsync(CancellationToken cancellationToken = default){       
        if(_userService.IdAccount != null){
            Result<string> resultGetAccessTokenSilently = await _authenticationService.GetAccessTokenSilentlyAsync();
            if(resultGetAccessTokenSilently.IsFailed){
                return;
            }

            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
    }

    [RelayCommand]
    private async Task LoginAsync()
    {

        Result<string> authenticationResult = await _authenticationService.AuthenticateWithoutProviderAsync(Email!, Password!);
        if(authenticationResult.IsFailed)
        {
            if(authenticationResult.HasError<AuthenticationService.IncorrectCredentialsError>()){
                IToast failedToast = Toast.Make("Incorrect credentials");
                await failedToast.Show();
            }
            else {
                IToast failedToast = Toast.Make("Unknown error");
                await failedToast.Show();
            }

            return;
        }

        Result<Claims> resultClaims = await _accountService.GetClaimsAsync();
        if(resultClaims.IsFailed)
        {
            IToast failedToast = Toast.Make("Unknown error when getting claims");
            await failedToast.Show();

            return;
        }

        _userService.IdAccount = resultClaims.Value.IdAccount;
        _userService.IsExternal = resultClaims.Value.IsExternal;

        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }

    [RelayCommand]
    private async Task LoginWithGoogleAsync()
    {
        Result<string> authenticationResult = await _authenticationService.AuthenticateViaProviderAsync("Google");

        if(authenticationResult.IsFailed)
        {
            IToast failedToast = Toast.Make("Unknown error when loggign with Google");
            await failedToast.Show();

            return;
        }

        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }

    [RelayCommand]
    private async Task CreateAccountAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(SignUpPage)}");
    }
}
