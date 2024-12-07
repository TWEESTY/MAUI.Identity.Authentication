using MyApplication.MAUI.ViewModels;

namespace MyApplication.MAUI.Pages;

public partial class SignUpPage : ContentPage
{
	private readonly SignUpPageViewModel _viewModel;
	public SignUpPage(SignUpPageViewModel viewModel)
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