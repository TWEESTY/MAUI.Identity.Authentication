using FluentResults;
using MediatR;
using MyApplication.Server.Data;

namespace MyApplication.Server.Features.Accounts.Queries;

public class GetClaimsAccountQueryHandler : IRequestHandler<GetClaimsAccountQuery, Result<GetClaimsAccountQueryResult>>
{    
    public Task<Result<GetClaimsAccountQueryResult>> Handle(GetClaimsAccountQuery request, CancellationToken cancellationToken)
    {        
        return Task.FromResult(Result.Ok(new GetClaimsAccountQueryResult(
            IdAccount: request.User.Id,
            IsExternal: string.IsNullOrEmpty(request.User.PasswordHash)
        )));
    }
}

public record GetClaimsAccountQuery(ApplicationUser User) : IRequest<Result<GetClaimsAccountQueryResult>>;

public record GetClaimsAccountQueryResult(string IdAccount, bool IsExternal);
