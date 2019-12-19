using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Shared;
using App.Shared.AuthModels;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace App.Client.Services
{
    public class AuthServiceHttpClient : AuthenticationStateProvider, IAuthService
    {
        private const string UserIdentityKey = "userIdentity";
        private readonly HttpClient _httpClient;
        private readonly LocalStorage _localStorage;

        public AuthServiceHttpClient(HttpClient httpClient, LocalStorage localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<SingInResult>(UserIdentityKey);

            if (savedToken == null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken.AccessToken);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, savedToken.Username),
            }, "jwt")));
        }
        
        public async Task SignIn(string username, string password)
        {
            var result = await _httpClient.PostJsonAsync<SingInResult>("api/auth/sign-in", new UserCredentials
            {
                Username = username,
                Password = password
            });
            if (result.Success)
            {
                await _localStorage.SetItemAsync(UserIdentityKey, result);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }
        }

        public async Task SignOut()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            await _localStorage.RemoveItemAsync(UserIdentityKey);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
