using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator.Client
{
    public class ClientMediator : IMediator
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClientMediator> _logger;

        private readonly Dictionary<int, Task> _queryTaskCache = new Dictionary<int, Task>();
        private readonly object _queryTaskCacheLock = new object();

        public ClientMediator(HttpClient httpClient, ILogger<ClientMediator> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IMediatorResponse> Fire(IEvent request, CancellationToken cancellationToken = default)
        {
            var contract = CreateContract(request);

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

        public async Task<IMediatorResponse<TResponse>> Execute<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var contract = CreateContract(request);

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

        private Task<IMediatorResponse<TResponse>> GetRequestTaskFromCacheOrCreateNewRequest<TResponse>(int hashCode, MediatorRequest contract, CancellationToken cancellationToken = default)
        {
            lock (_queryTaskCacheLock)
            {
                if (_queryTaskCache.TryGetValue(hashCode, out var task))
                {
                    return (Task<IMediatorResponse<TResponse>>)task;
                }
                var newTask = SendRequest<TResponse>(contract, cancellationToken);
                _queryTaskCache[hashCode] = newTask;
                return newTask;
            }
        }

        private async Task<IMediatorResponse<TResponse>> SendRequest<TResponse>(MediatorRequest contract, CancellationToken cancellationToken = default)
        {
            var typeName = typeof(IRequest<TResponse>);
            try
            {
                var url = MediatorRequest.Endpoint + $"?type={typeName}";
                var response = await _httpClient.PostAsJsonAsync(url, contract, cancellationToken);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<MediatorResponseDeserialized<TResponse>>(cancellationToken: cancellationToken);
                return result?? throw new InvalidOperationException("No data received");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Mediator request failed for "+ typeName);
                return new MediatorResponse<TResponse>(e.Message);
            }
        }

        private MediatorRequest CreateContract(object request)
        {
            return new MediatorRequest
            {
                Json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }),
                ObjectName = request.GetType().AssemblyQualifiedName
            };
        }
    }
}
