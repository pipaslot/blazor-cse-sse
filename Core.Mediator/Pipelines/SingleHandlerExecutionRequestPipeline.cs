using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;

namespace Core.Mediator.Pipelines
{
    /// <summary>
    /// Pipeline executing one handler for request implementing TMarker type
    /// </summary>
    public class SingleHandlerExecutionRequestPipeline : BaseRequestPipeline
    {
        public SingleHandlerExecutionRequestPipeline(HandlerResolver handlerResolver) : base(handlerResolver)
        {
        }

        public override async Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var handlers = GetRegisteredHandlers<TRequest, TResponse>(request);
            if (handlers.Length > 1)
            {
                throw new Exception($"Multiple handlers were registered for the same request. Remove one from defined type: {string.Join(" OR ", handlers)}");
            }

            var handler = handlers.FirstOrDefault();
            if (handler == null)
            {
                throw new Exception("No handler was found for " + request.GetType());
            }
            return await Execute<TRequest, TResponse>(handler, request, cancellationToken);
        }
    }
}