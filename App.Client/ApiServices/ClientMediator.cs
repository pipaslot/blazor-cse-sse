﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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

        public async Task<MediatorResponse> Fire(IEvent request, CancellationToken cancellationToken = default)
        {
            var contract = RequestContractFactory.Create(request);

            var hashCode = (contract.Json, contract.ObjectName).GetHashCode();
            try
            {
                var task = GetRequestTaskFromCacheOrCreateNewRequest<object>(hashCode, contract, cancellationToken);
                return await task;
            }
            finally
            {
                lock (_queryTaskCacheLock)
                {
                    _queryTaskCache.Remove(hashCode);
                }
            }
        }

        public async Task<MediatorResponse<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var contract = RequestContractFactory.Create(request);

            var hashCode = (contract.Json, contract.ObjectName).GetHashCode();
            try
            {
                var task = GetRequestTaskFromCacheOrCreateNewRequest<TResponse>(hashCode, contract, cancellationToken);
                return await task;
            }
            finally
            {
                lock (_queryTaskCacheLock)
                {
                    _queryTaskCache.Remove(hashCode);
                }
            }
        }

        private Task<MediatorResponse<TResponse>> GetRequestTaskFromCacheOrCreateNewRequest<TResponse>(int hashCode, RequestContract contract, CancellationToken cancellationToken = default)
        {
            lock (_queryTaskCacheLock)
            {
                if (_queryTaskCache.TryGetValue(hashCode, out var task))
                {
                    return (Task<MediatorResponse<TResponse>>)task;
                }
                var newTask = SendRequest<TResponse>(contract, cancellationToken);
                _queryTaskCache[hashCode] = newTask;
                return newTask;
            }
        }

        private async Task<MediatorResponse<TResponse>> SendRequest<TResponse>(RequestContract contract, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = "api/mediator/request?type=" + typeof(IRequest<TResponse>).FullName;
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
    }
}