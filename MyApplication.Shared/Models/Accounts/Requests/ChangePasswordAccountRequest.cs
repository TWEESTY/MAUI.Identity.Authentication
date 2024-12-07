namespace MyApplication.Shared.Models.Accounts.Requests;

public record ChangePasswordAccountRequest(string OldPassword, string NewPassword);
