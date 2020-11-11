using System;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Mediator;
using Microsoft.JSInterop;

namespace App.Server.Services
{
    public class SaveServerMediator : IMediator
    {
        private readonly ServerMediator _mediator;
        private readonly IJSRuntime _jsRuntime;

        public SaveServerMediator(ServerMediator mediator, IJSRuntime jsRuntime)
        {
            _mediator = mediator;
            _jsRuntime = jsRuntime;
        }

        public async Task<MediatorResponse<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            try
            {
                return  await _mediator.Send(query, cancellationToken);
            }
            catch (Exception e)
            {
                await _jsRuntime.InvokeAsync<string>("alert", cancellationToken, "Server error: "+e.Message);
                return new MediatorResponse<TResponse>(e.Message);
            }
        }

        public async Task<MediatorResponse> Publish<TCommand>(TCommand command, CancellationToken cancellationToken = default)where TCommand : ICommand
        {
            try
            {
                return await _mediator.Publish(command, cancellationToken);
            }
            catch (Exception e)
            {
                await _jsRuntime.InvokeAsync<string>("alert", cancellationToken, "Server error: "+e.Message);
                return new MediatorResponse(e.Message);
            }
        }
    }
}