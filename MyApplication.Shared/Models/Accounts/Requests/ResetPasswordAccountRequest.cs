namespace MyApplication.Shared.Models.Accounts.Requests;

public record ResetPasswordAccountRequest(string Email, string NewPassword, string ResetCode);