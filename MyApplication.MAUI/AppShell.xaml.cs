using MyApplication.MAUI.Pages;

namespace MyApplication.MAUI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		RegisterRoutes();
	}

	public void RegisterRoutes(){
		Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
		Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
	}
}
