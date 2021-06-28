using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;

namespace Core.Mediator.Pipelines
{
    /// <summary>
    /// Pipeline executing multiple handlers implementing TMarker type. All handlers are executed asynchronously at the same time
    /// </summary>
    public class MultiHandlerConcurrentExecutionEventPipeline : BaseEventPipeline , IExecutivePipeline
    {
        public MultiHandlerConcurrentExecutionEventPipeline(ServiceResolver handlerResolver) : base(handlerResolver)
        {
        }

        public bool ExecuteMultipleHandlers => true;

        public override async Task Handle<TEvent>(TEvent request, CancellationToken cancellationToken, EventHandlerDelegate next)
        {
            var handlers = GetRegisteredHandlers(request);
            if (handlers.Length == 0)
            {
                throw new Exception("No handler was found for " + request.GetType());
            }

            var tasks = handlers
                .Select(handler => Execute(handler, request, cancellationToken))
                .ToArray();
            await Task.WhenAll(tasks);
        }
    }
}