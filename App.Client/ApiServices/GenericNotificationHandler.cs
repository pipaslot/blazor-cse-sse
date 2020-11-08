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
    public class GenericNotificationHandler<TRequest> : INotificationHandler<TRequest> where TRequest : INotification
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public GenericNotificationHandler(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task Handle(TRequest request, CancellationToken cancellationToken)
        {
            
            try
            {
                await _httpClient.PostJsonAsync("api/mediator/notification?type="+typeof(TRequest).FullName, new RequestNotificationContract(request));
            }
            catch (Exception e)
            {
                await _jsRuntime.InvokeAsync<string>("alert", "Server error: "+e.Message);
                throw;
            }
        }
    }
}