using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator;
using Microsoft.JSInterop;

namespace App.Server.MediatorPipelines
{
    public class ExceptionHandlerQueryPipeline<TQuery, TResponse> : IQueryPipeline<TQuery, TResponse> where TQuery: notnull
    {
        private readonly IJSRuntime _jsRuntime;

        public ExceptionHandlerQueryPipeline(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken, QueryHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();
            }
            catch (Exception e)
            {
                await _jsRuntime.InvokeAsync<string>("alert", cancellationToken, "Server error: "+e.Message);
                return default!;
            }
        }
    }

}