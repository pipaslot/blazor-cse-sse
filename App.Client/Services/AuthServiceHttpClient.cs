using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using App.Shared;
using App.Shared.AuthModels;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Components.States;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace App.Client.Services
{
    public class AuthServiceHttpClient : AuthenticationStateProvider, IAuthService
    {
        private const string AuthTokenKey = "userAuthToken";
        private readonly HttpClient _httpClient;
        private readonly LocalStorage _localStorage;

        public AuthServiceHttpClient(HttpClient httpClient, LocalStorage localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync(AuthTokenKey);

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt")));
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
                await _localStorage.SetItemAsync(AuthTokenKey, result.AccessToken);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }
        }

        public async Task SignOut()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            await _localStorage.RemoveItemAsync(AuthTokenKey);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            
            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
