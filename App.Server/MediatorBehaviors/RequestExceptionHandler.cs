using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Microsoft.JSInterop;

namespace App.Server.MediatorBehaviors
{
    public class RequestExceptionHandler<TRequest, TResponse, TException> : MediatR.Pipeline.RequestExceptionAction<TRequest, TException>where TRequest : notnull
        where TException : Exception
    {
        private readonly IJSRuntime _jsRuntime;

        public RequestExceptionHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        protected override void Execute(TRequest request, TException exception)
        {
            _jsRuntime.InvokeAsync<string>("alert", "Server error: " + exception.Message).ConfigureAwait(false);
        }
    }
}
