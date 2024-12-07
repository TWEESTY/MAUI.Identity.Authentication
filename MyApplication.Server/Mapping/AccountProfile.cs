using AutoMapper;
using MyApplication.Server.Features.Accounts.Queries;
using MyApplication.Shared.Models.Accounts.Results;

namespace MyApplication.Server.Mapping;

public class AccountProfile : Profile
{
    public AccountProfile(){
        CreateMap<GetClaimsAccountQueryResult, GetClaimsAccountResult>();
    }
}
