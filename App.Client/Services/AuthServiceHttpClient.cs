using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Auth;
using Microsoft.AspNetCore.Components;

namespace App.Client.Services
{
    public class AuthServiceHttpClient : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string[]> GetUserPermissions()
        {
            return await _httpClient.GetJsonAsync<string[]>("api/auth");
        }
    }
}
