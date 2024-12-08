using AutoMapper;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyApplication.Server.Data;
using MyApplication.Server.Features.Accounts.Commands;
using MyApplication.Server.Features.Accounts.Queries;
using MyApplication.Shared.Models.Accounts.Requests;
using MyApplication.Shared.Models.Accounts.Results;

namespace MyApplication.Server.Controllers;

[ApiController]
public class AccountsControllers(
    IMediator mediator,
    IMapper mapper, 
    UserManager<ApplicationUser> userManager): ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly IMediator _mediator = mediator;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [HttpPost("api/accounts")]
    [EndpointDescription("Create an account")]
    [EndpointSummary("Create an account")]
    [EndpointName("Account_Create")]
    [Tags("AccountsEndpoint")]
    public async Task<ActionResult> CreateAccountAsync([FromBody] CreateAccountRequest request, CancellationToken cancellationToken = default){
        Result result = await _mediator.Send(
            new CreateAccountCommand(
                request.Email,
                request.Password,
                UrlForConfirmEmail: $"{Request.Scheme}://{Request.Host}/Account/ConfirmEmail")
        );
        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("api/accounts/claims")]
    [EndpointDescription("Get the claims of the current user")]
    [EndpointSummary("Get the claims of the current user")]
    [EndpointName("Account_GetClaims")]
    [Tags("AccountsEndpoint")]
    public async Task<ActionResult<GetClaimsAccountResult>> GetClaimsAccountAsync(CancellationToken cancellationToken = default){
        ApplicationUser user = (await _userManager.GetUserAsync(HttpContext.User))!;
        Result<GetClaimsAccountQueryResult> result = await _mediator.Send(
            new GetClaimsAccountQuery(user)
        );
        return result.Map(this._mapper.Map<GetClaimsAccountResult>).ToActionResult();   
    }
}
