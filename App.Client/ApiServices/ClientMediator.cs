using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace App.Client.ApiServices
{
    public class ClientMediator : IMediator
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILogger<ClientMediator> _logger;

        private readonly Dictionary<int, Task> _queryTaskCache = new Dictionary<int, Task>();
        private readonly object _queryTaskCacheLock = new object();

        public ClientMediator(HttpClient httpClient, IJSRuntime jsRuntime, ILogger<ClientMediator> logger)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _logger = logger;
        }

        public async Task<MediatorResponse<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            var contract = new CommandQueryContract(query);

            var task = GetQueryTaskFromCacheOrCreateNewRequest<TResponse>(contract, cancellationToken);
            return await task;
        }

        private Task<MediatorResponse<TResponse>> GetQueryTaskFromCacheOrCreateNewRequest<TResponse>(CommandQueryContract contract, CancellationToken cancellationToken = default)
        {
            var hashCode = (contract.Json, contract.ObjectName).GetHashCode();
            lock (_queryTaskCacheLock)
            {
                if (_queryTaskCache.TryGetValue(hashCode, out var task))
                {
                    return (Task<MediatorResponse<TResponse>>)task;
                }
                var newTask = SendQuery<TResponse>(contract, cancellationToken);
                _queryTaskCache[hashCode] = newTask;
                return newTask;
            }
        }

        private async Task<MediatorResponse<TResponse>> SendQuery<TResponse>(CommandQueryContract contract, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = "api/mediator/query?type=" + typeof(IQuery<TResponse>).FullName;
                var response = await _httpClient.PostAsJsonAsync(url, contract, cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<MediatorResponse<TResponse>>(cancellationToken: cancellationToken)
                    ?? throw new InvalidOperationException("No data received");
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
            var contract = new CommandQueryContract(command);
            try
            {
                var url = "api/mediator/command?type=" + typeof(TCommand).FullName;
                var response = await _httpClient.PostAsJsonAsync(url, contract, cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<MediatorResponse>(cancellationToken: cancellationToken)
                    ?? throw new InvalidOperationException("No data received");
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