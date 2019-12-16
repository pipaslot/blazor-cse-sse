using System;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Configuration;
using Microsoft.AspNetCore.Components;

namespace App.Client
{
    /// <summary>
    /// Connects to server side and reads section from appSettings.json files
    /// </summary>
    public class HttpClientConfigProvider : IConfigProvider
    {
        private readonly HttpClient _httpClient;
        private Config _config;
        private Task<Config> _task;

        public HttpClientConfigProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Config> GetConfig()
        {
            if (_config == null)
            {
                //Prevent multiple parallel calls
                if (_task == null)
                {
                    _task = _httpClient.GetJsonAsync<Config>("/config.json?v=" + DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss"));
                }
                _config = await _task;
            }

            return _config;
        }
    }
}
