using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator;
using Microsoft.JSInterop;

namespace App.Server.MediatorPipelines
{
    public class ExceptionHandlerCommandPipeline<TCommand> : ICommandPipeline<TCommand> where TCommand: notnull
    {
        private readonly IJSRuntime _jsRuntime;

        public ExceptionHandlerCommandPipeline(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task Handle(TCommand query, CancellationToken cancellationToken, CommandHandlerDelegate next)
        {
            try
            {
                await next();
            }
            catch (Exception e)
            {
                await _jsRuntime.InvokeAsync<string>("alert", cancellationToken, "Server error: "+e.Message);
            }
        }
    }

}