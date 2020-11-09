using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using App.Shared;
using App.Shared.SafeMediator;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace App.Client.ApiServices
{
    public class SaveClientMediator : App.Shared.SafeMediator.IMediator
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public SaveClientMediator(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task<MediatorResponse<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostJsonAsync<TResponse>("api/mediator/request?type=" + typeof(IRequest<TResponse>).FullName, new RequestNotificationContract(request));
                return new MediatorResponse<TResponse>(response);
            }
            catch (Exception e)
            {
                await _jsRuntime.InvokeAsync<string>("alert", cancellationToken, "Server error: "+e.Message);
                return new MediatorResponse<TResponse>(e.Message);
            }
        }

        public async Task<MediatorResponse> Publish<TNotification>(TNotification request, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            try
            {
                await _httpClient.PostJsonAsync("api/mediator/notification?type="+typeof(TNotification).FullName, new RequestNotificationContract(request));
                return new MediatorResponse();
            }
            catch (Exception e)
            {
                await _jsRuntime.InvokeAsync<string>("alert", "Server error: "+e.Message);
                return new MediatorResponse(e.Message);
            }
        }
    }
}