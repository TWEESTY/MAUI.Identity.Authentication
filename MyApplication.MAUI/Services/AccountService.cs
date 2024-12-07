using FluentResults;
using MyApplication.MAUI.Models;
using MyApplication.MAUI.Services.Interfaces;
using MyApplication.Shared.Models.Accounts.Requests;
using MyApplication.Shared.Models.Accounts.Results;
using MyApplication.Shared.Services.Interfaces;

namespace MyApplication.MAUI.Services;

public class AccountService : IAccountService
{

    private const string IdAccountKeyForStorage = "id_account";
    private const string IsExternalKeyForStorage = "is_external_account";

    private readonly IAccountProviderService _accountProviderService;
    private readonly IUserService _userService;

    public AccountService(IAccountProviderService accountProviderService, IUserService userService)
    {
        _accountProviderService = accountProviderService;
        _userService = userService;
    }

    /// <summary>
    /// Gets the claims of the current user.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<Claims>> GetClaimsAsync(CancellationToken cancellationToken = default){
        try{            
            Result<GetClaimsAccountResult> getClaimsRequestResult = await _accountProviderService.GetClaimsAccountAsync(cancellationToken);

            if(getClaimsRequestResult.IsFailed){
                return Result.Fail("Failed to get claims");
            }

            await StoreClaimsAsync(
                idAccount: getClaimsRequestResult.Value.IdAccount,
                isExternal: getClaimsRequestResult.Value.IsExternal
            );

            return Result.Ok(new Claims(IdAccount: getClaimsRequestResult.Value.IdAccount, IsExternal:getClaimsRequestResult.Value.IsExternal));
        }
        catch (TaskCanceledException){
            return Result.Fail("Task was canceled");
        }
        catch (Exception){
            return Result.Fail("Failed to get claims via provider");
        }
    }

    public Task<Result> CreateAccountAsync(CreateAccountRequest request, CancellationToken cancellationToken = default){
         return _accountProviderService.CreateAccountAsync(request, cancellationToken);
    }

    public Task<Result> ForgotPasswordAccountAsync(string email, CancellationToken cancellationToken = default){
        return _accountProviderService.ForgotPasswordAccountAsync(new ForgotPasswordAccountRequest(email), cancellationToken);
    }

    public Task<Result> ChangePasswordAccountAsync(ChangePasswordAccountRequest request, CancellationToken cancellationToken = default){
         return _accountProviderService.ChangePasswordAccountAsync(request, cancellationToken);
    }

    private async Task StoreClaimsAsync(string idAccount, bool isExternal){

        await SecureStorage.SetAsync(IdAccountKeyForStorage, idAccount);
        await SecureStorage.SetAsync(IsExternalKeyForStorage, isExternal.ToString());
    }

    public Task<Result> DeleteAccountAsync(CancellationToken cancellationToken = default){
        return _accountProviderService.DeleteAccountAsync(
            idAccount: _userService.IdAccount!,
            cancellationToken);
    }
}
