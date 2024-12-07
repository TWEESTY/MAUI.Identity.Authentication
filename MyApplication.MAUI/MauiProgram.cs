using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MyApplication.MAUI.Handlers;
using MyApplication.MAUI.Helpers;
using MyApplication.MAUI.Pages;
using MyApplication.MAUI.Services;
using MyApplication.MAUI.Services.Interfaces;
using MyApplication.MAUI.ViewModels;
using MyApplication.Shared.Services;
using MyApplication.Shared.Services.Interfaces;

namespace MyApplication.MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		ConfigureHttpServices(builder);
		ConfigureServices(builder);
		ConfigurePagesAndViewModels(builder);

		return builder.Build();
	}

	private static void ConfigureHttpServices(MauiAppBuilder builder)
	{
		builder.Services.AddKeyedSingleton("UnauthenticatedHttpClient", (serviceProvider, serviceKey) => {
			var handler = new HttpClientHandler();
			handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
			{
				if (cert != null && cert.Issuer.Equals("CN=localhost"))
					return true;
				return errors == System.Net.Security.SslPolicyErrors.None;
			};

			var httpClient = new HttpClient(handler);
			httpClient.Timeout = TimeSpan.FromSeconds(30);
			httpClient.BaseAddress = new Uri(UrlHelper.ServerBaseAddress);
			return httpClient;
		});

		builder.Services.AddKeyedSingleton("AuthenticatedHttpClient", (serviceProvider, serviceKey) => {
			var handler = new HttpClientHandler();
			handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
			{
				if (cert != null && cert.Issuer.Equals("CN=localhost"))
					return true;
				return errors == System.Net.Security.SslPolicyErrors.None;
			};

			var httpClient = new HttpClient(
				new AuthenticationDelegatingHandler(
					serviceProvider.GetRequiredService<IAuthenticationService>(),
					handler
				)
			);
			httpClient.Timeout = TimeSpan.FromSeconds(30);
			httpClient.BaseAddress = new Uri(UrlHelper.ServerBaseAddress);
			return httpClient;
		});
	}

	private static void ConfigureServices(MauiAppBuilder builder)
	{
		builder.Services.AddSingleton<IUserService, UserService>();

		builder.Services.AddSingleton<IAuthenticationService>(serviceProvider => {
			return new AuthenticationService(serviceProvider.GetKeyedService<HttpClient>("UnauthenticatedHttpClient")!, serviceProvider.GetService<IUserService>()!);
		});

		builder.Services.AddSingleton<IAccountProviderService>(serviceProvider => {
			return new ClientAccountProviderService(
				serviceProvider.GetKeyedService<HttpClient>("AuthenticatedHttpClient")!,
				serviceProvider.GetKeyedService<HttpClient>("UnauthenticatedHttpClient")!
			);
		});



		builder.Services.AddSingleton<IAccountService, AccountService>();
	}

	private static void ConfigurePagesAndViewModels(MauiAppBuilder builder)
	{
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<MainPageViewModel>();

		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<LoginPageViewModel>();

		builder.Services.AddTransient<SignUpPage>();
		builder.Services.AddTransient<SignUpPageViewModel>();
	}
}
