using MyApplication.MAUI.ViewModels;

namespace MyApplication.MAUI.Pages;

public partial class LoginPage : ContentPage
{
	private readonly LoginPageViewModel _viewModel;
	public LoginPage(LoginPageViewModel viewModel)
	{
		BindingContext = viewModel;
		_viewModel = viewModel;
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
	}
}