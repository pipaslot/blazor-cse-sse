using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;

namespace Core.Mediator.Middlewares
{
    /// <summary>
    /// Pipeline executing multiple handlers implementing TMarker type. Handlers are executed in row, once previous execution finished.
    /// </summary>
    public class MultiHandlerSequenceExecutionEventMiddleware : BaseEventMiddleware, IExecutiveMiddleware
    {
        public MultiHandlerSequenceExecutionEventMiddleware(ServiceResolver handlerResolver) : base(handlerResolver)
        {
        }
        public bool ExecuteMultipleHandlers => true;

        public override async Task Handle<TEvent>(TEvent request, CancellationToken cancellationToken, MiddlewareDelegate next)
        {
            var handlers = GetRegisteredHandlers(request);
            if (handlers.Length == 0)
            {
                throw new Exception("No handler was found for " + request.GetType());
            }
            foreach (var handler in handlers)
            {
                await Execute(handler, request, cancellationToken);
            }
        }
    }
}