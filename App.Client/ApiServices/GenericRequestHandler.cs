using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using App.Shared;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace App.Client.ApiServices
{
    public class GenericRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly HttpClient _httpClient;

        public GenericRequestHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            return await _httpClient.PostJsonAsync<TResponse>("api/mediator/request", new RequestNotificationContract(request));
        }
    }
}