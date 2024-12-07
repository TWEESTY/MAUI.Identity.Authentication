namespace MyApplication.Shared.Models.MobileAuthentication.Results;

public record AuthenticateMobileRequestResult(string AccessToken, string RefreshToken, int ExpiresIn);
