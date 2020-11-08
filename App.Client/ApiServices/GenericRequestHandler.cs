using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using App.Shared;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace App.Client.ApiServices
{
    public class GenericRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public GenericRequestHandler(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _httpClient.PostJsonAsync<TResponse>(
                    "api/mediator/request?type=" + typeof(TRequest).FullName, new RequestNotificationContract(request));
            }
            catch (Exception e)
            {
                await _jsRuntime.InvokeAsync<string>("alert", "Server error: "+e.Message);
                throw;
            }
        }
    }
}