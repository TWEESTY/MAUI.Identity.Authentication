namespace MyApplication.MAUI.Helpers;

public static class UrlHelper
{
	public static string ServerBaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "https://myapp.local.com:7046" : "https://localhost:7046";
}