using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Client.Store;
using App.Shared.Auth;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Pipaslot.Mediator.Abstractions;

namespace App.Client.Services
{
    public class AuthService : AuthenticationStateProvider, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IMediator _mediator;
        private readonly IDispatcher _dispatcher;
        private readonly IState<Authentication.State> _authenticationState;

        public AuthService(HttpClient httpClient, IDispatcher dispatcher, IState<Authentication.State> authenticationState, IMediator mediator)
        {
            _httpClient = httpClient;
            _dispatcher = dispatcher;
            _authenticationState = authenticationState;
            _authenticationState.StateChanged += AuthenticationState_StateChanged;
            _mediator = mediator;
        }

        private void AuthenticationState_StateChanged(object? sender, Authentication.State state)
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authState = _authenticationState.Value;
            if (!authState.IsAuthenticated)
            {
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            }

            if (authState.BearerToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", authState.BearerToken.Value);
            }

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, authState.UserName),
            }, "jwt"))));
        }

        public async Task SignIn(string username, string password)
        {
            var response = await _mediator.Send(new SignInRequest.Query
            {
                Username = username,
                Password = password
            });
            if (!response.Success)
            {
                throw new Exception("Authentication request failed");
            }
            _dispatcher.Dispatch(new Authentication.SignInAction(response.Result.AccessToken, response.Result.Username));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", response.Result.AccessToken.Value);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        }

        public Task SignOut()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _dispatcher.Dispatch(new Authentication.SignOutAction());
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _authenticationState.StateChanged -= AuthenticationState_StateChanged;
        }
    }
}