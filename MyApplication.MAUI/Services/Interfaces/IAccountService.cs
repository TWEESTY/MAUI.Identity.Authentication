using FluentResults;
using MyApplication.MAUI.Models;
using MyApplication.Shared.Models.Accounts.Requests;

namespace MyApplication.MAUI.Services.Interfaces;

public interface IAccountService
{
    public Task<Result<Claims>> GetClaimsAsync(CancellationToken cancellationToken = default);
    public Task<Result> CreateAccountAsync(CreateAccountRequest request, CancellationToken cancellationToken = default);
    public Task<Result> ForgotPasswordAccountAsync(string email, CancellationToken cancellationToken = default);
    public Task<Result> DeleteAccountAsync(CancellationToken cancellationToken = default);
    public Task<Result> ChangePasswordAccountAsync(ChangePasswordAccountRequest request, CancellationToken cancellationToken = default);
}
