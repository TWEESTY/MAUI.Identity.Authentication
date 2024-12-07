using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MyApplication.MAUI.Pages;
using MyApplication.MAUI.Services.Interfaces;
using MyApplication.Shared.Models.Accounts.Requests;

namespace MyApplication.MAUI.ViewModels;

public partial class SignUpPageViewModel(
    IAuthenticationService authenticationService,
    IAccountService accountService) : ObservableObject
{
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IAccountService _accountService = accountService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [RelayCommand]
    private async Task SignUpAsync()
    {
        Result accountCreationResult = await _accountService.CreateAccountAsync(new CreateAccountRequest(Email!, Password!));
        if(accountCreationResult.IsFailed)
        {
            IToast failedToast = Toast.Make("Failed to create account");
            await failedToast.Show();
            return;
        }
        IToast toast = Toast.Make("Account created successfully. Confirm your email to login.");
        await toast.Show();

        await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
    }

    [RelayCommand]
    private async Task SignUpWithGoogleAsync()
    {
        Result<string> authenticationResult = await _authenticationService.AuthenticateViaProviderAsync("Google");

        if(authenticationResult.IsFailed)
        {
            IToast failedToast = Toast.Make("Unknown error when loggign with Google");
            await failedToast.Show();

            return;
        }

        await Shell.Current.GoToAsync($"{nameof(MainPage)}");
    }
}
