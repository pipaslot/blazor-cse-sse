using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;

namespace Core.Mediator.Middlewares
{
    /// <summary>
    /// Pipeline executing multiple handlers implementing TMarker type. Handlers are executed in row, once previous execution finished.
    /// </summary>
    public class MultiHandlerSequenceExecutionMiddleware : ExecutionMiddleware
    {
        private readonly ServiceResolver _handlerResolver;

        public MultiHandlerSequenceExecutionMiddleware(ServiceResolver handlerResolver)
        {
            _handlerResolver = handlerResolver;
        }

        public override bool ExecuteMultipleHandlers => true;

        protected override async Task HandleMessage<TMessage>(TMessage message, CancellationToken cancellationToken)
        {
            var handlers = _handlerResolver.GetMessageHandlers(message?.GetType());
            if (handlers.Length == 0)
            {
                throw new Exception("No handler was found for " + message?.GetType());
            }
            foreach (var handler in handlers)
            {
                await ExecuteMessage(handler, message, cancellationToken);
            }
        }
        protected override async Task HandleRequest<TRequest>(TRequest request, MediatorResponse response, CancellationToken cancellationToken)
        {
            var resultType = GenericHelpers.GetRequestResultType(request?.GetType());
            var handlers = _handlerResolver.GetRequestHandlers(request?.GetType(), resultType);
            if (handlers.Length == 0)
            {
                throw new Exception("No handler was found for " + request?.GetType());
            }
            foreach (var handler in handlers)
            {
                await ExecuteRequest(handler, request, response, cancellationToken);
            }
        }
    }
}