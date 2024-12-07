using System.Net.Http.Headers;
using FluentResults;
using MyApplication.MAUI.Exceptions;
using MyApplication.MAUI.Services.Interfaces;

namespace MyApplication.MAUI.Handlers;

public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationDelegatingHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public AuthenticationDelegatingHandler(IAuthenticationService authenticationService, HttpMessageHandler innerHandler)
        : base(innerHandler)
    {
        _authenticationService = authenticationService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Result<string> resultGetAccessToken = await _authenticationService.GetAccessTokenSilentlyAsync();
        if (resultGetAccessToken.IsFailed)
        {
            throw new NotAuthenticatedException();
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", resultGetAccessToken.Value);
        
        return await base.SendAsync(request, cancellationToken);
    }
}
