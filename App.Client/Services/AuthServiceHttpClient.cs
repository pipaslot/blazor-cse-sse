using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Shared;
using App.Shared.AuthModels;
using Components.Store;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace App.Client.Services
{
    public class AuthServiceHttpClient : AuthenticationStateProvider, IAuthService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IDispatcher _dispatcher;
        private readonly IState<Authentication.State> _authenticationState;

        public AuthServiceHttpClient(HttpClient httpClient, IDispatcher dispatcher, IState<Authentication.State> authenticationState)
        {
            _httpClient = httpClient;
            _dispatcher = dispatcher;
            _authenticationState = authenticationState;
            _authenticationState.StateChanged += AuthenticationState_StateChanged;
        }

        private void AuthenticationState_StateChanged(object sender, Authentication.State state)
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authState = _authenticationState.Value;
            if (!authState.IsAuthenticated){
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            }

            if (authState.BearerToken != null){
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", authState.BearerToken.Value);
            }

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, authState.UserName),
            }, "jwt"))));
        }

        public async Task SignIn(string username, string password)
        {
            var result = await _httpClient.PostJsonAsync<SingInResult>("api/auth/sign-in", new UserCredentials
            {
                Username = username,
                Password = password
            });
            if (result.Success){
                _dispatcher.Dispatch(new Authentication.SignInAction(result.AccessToken, result.Username));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken.Value);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }
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