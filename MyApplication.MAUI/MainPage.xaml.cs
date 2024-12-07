using MyApplication.MAUI.ViewModels;

namespace MyApplication.MAUI;

public partial class MainPage : ContentPage
{
	private readonly MainPageViewModel _viewModel;
	public MainPage(MainPageViewModel viewModel)
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

