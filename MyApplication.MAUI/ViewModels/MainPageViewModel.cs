using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApplication.MAUI.Pages;
using MyApplication.MAUI.Services.Interfaces;

namespace MyApplication.MAUI.ViewModels;

public partial class MainPageViewModel(IUserService userService) : ObservableObject
{
    private readonly IUserService _userService = userService;
    

    [RelayCommand]
    private async Task LogoutAsync()
    {
        _userService.ClearAllInformation();        
        await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
    }
}
