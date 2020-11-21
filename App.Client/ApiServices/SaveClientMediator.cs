using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace App.Client.ApiServices
{
    public class SaveClientMediator : IMediator
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public SaveClientMediator(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }
        
        public async Task<MediatorResponse<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostJsonAsync<TResponse>("api/mediator/query?type=" + typeof(IQuery<TResponse>).FullName, new RequestNotificationContract(query));
                return new MediatorResponse<TResponse>(response);
            }
            catch (Exception e)
            {
                await _jsRuntime.InvokeAsync<string>("alert", cancellationToken, "Server error: "+e.Message);
                return new MediatorResponse<TResponse>(e.Message);
            }
        }

        public async Task<MediatorResponse> Dispatch<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            try
            {
                await _httpClient.PostJsonAsync("api/mediator/command?type="+typeof(TCommand).FullName, new RequestNotificationContract(command));
                return new MediatorResponse();
            }
            catch (Exception e)
            {
                await _jsRuntime.InvokeAsync<string>("alert", cancellationToken, "Server error: "+e.Message);
                return new MediatorResponse(e.Message);
            }
        }
    }
}