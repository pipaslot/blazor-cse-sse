using System;
using System.Net.Http;
using System.Threading.Tasks;
using Components.Services;
using Microsoft.AspNetCore.Components;

namespace Optimiser.Blazor
{

    public class HttpClientConfigProvider : IConfigProvider
    {
        private readonly HttpClient _httpClient;
        private Config _config;

        public HttpClientConfigProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Config> GetConfig()
        {
            if (_config == null)
            {
                _config = await _httpClient.GetJsonAsync<Config>("/_content/Components/config.json?v=" + DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss"));
            }

            return _config;
        }
    }
}
