using FluentResults;
using MyApplication.Shared.Models.Accounts.Requests;
using MyApplication.Shared.Models.Accounts.Results;

namespace MyApplication.Shared.Services.Interfaces;

public interface IAccountProviderService
{
    Task<Result> CreateAccountAsync(CreateAccountRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAccountAsync(string idAccount, CancellationToken cancellationToken = default);
    Task<Result<GetClaimsAccountResult>> GetClaimsAccountAsync(CancellationToken cancellationToken = default);
    Task<Result> ReactivateAccountAsync(string idAccount, CancellationToken cancellationToken = default);
    Task<Result> SoftDeleteAccountAsync(string idAccount, CancellationToken cancellationToken = default);
    Task<Result> ForgotPasswordAccountAsync(ForgotPasswordAccountRequest request, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAccountAsync(ResetPasswordAccountRequest request, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAccountAsync(ChangePasswordAccountRequest request, CancellationToken cancellationToken = default);
}
