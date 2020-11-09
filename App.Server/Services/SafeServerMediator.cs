using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using App.Shared.SafeMediator;
using Microsoft.JSInterop;

namespace App.Server.Services
{
    public class SaveServerMediator : App.Shared.SafeMediator.IMediator
    {
        private readonly MediatR.IMediator _mediator;
        private readonly IJSRuntime _jsRuntime;

        public SaveServerMediator(MediatR.IMediator mediator, IJSRuntime jsRuntime)
        {
            _mediator = mediator;
            _jsRuntime = jsRuntime;
        }

        public async Task<MediatorResponse<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _mediator.Send(request, cancellationToken);
                return new MediatorResponse<TResponse>(response);
            }
            catch (Exception e)
            {
                await _jsRuntime.InvokeAsync<string>("alert", cancellationToken, "Server error: "+e.Message);
                return new MediatorResponse<TResponse>(e.Message);
            }
        }

        public async Task<MediatorResponse> Publish<TNotification>(TNotification request, CancellationToken cancellationToken = default)where TNotification : INotification
        {
            try
            {
                await _mediator.Publish(request, cancellationToken);
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