using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace App.Client.ApiServices
{
    public class ClientMediator : IMediator
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILogger<ClientMediator> _logger;

        public ClientMediator(HttpClient httpClient, IJSRuntime jsRuntime, ILogger<ClientMediator> logger)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _logger = logger;
        }
        
        public async Task<MediatorResponse<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostJsonAsync<MediatorResponse<TResponse>>("api/mediator/query?type=" + typeof(IQuery<TResponse>).FullName, new CommandQueryContract(query));
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Query failed");
                await _jsRuntime.InvokeAsync<string>("alert", cancellationToken, "Request failed");
                return new MediatorResponse<TResponse>(e.Message);
            }
        }

        public async Task<MediatorResponse> Dispatch<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            try
            {
                var response = await _httpClient.PostJsonAsync<MediatorResponse>("api/mediator/command?type="+typeof(TCommand).FullName, new CommandQueryContract(command));
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Command failed");
                await _jsRuntime.InvokeAsync<string>("alert", cancellationToken, "Request failed");
                return new MediatorResponse(e.Message);
            }
        }
    }
}