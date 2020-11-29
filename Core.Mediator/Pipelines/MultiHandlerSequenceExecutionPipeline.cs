using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;

namespace Core.Mediator.Pipelines
{
    /// <summary>
    /// Pipeline executing multiple handlers implementing TMarker type. Handlers are executed in row, once previous execution finished.
    /// </summary>
    public class MultiHandlerSequenceExecutionPipeline : BasePipeline
    {
        public MultiHandlerSequenceExecutionPipeline(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        
        public override async Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var handlers = GetRegisteredHandlers<TRequest, TResponse>(request);
            if (handlers.Length == 0)
            {
                throw new Exception("No handler was found for " + request.GetType());
            }
            foreach (var handler in handlers)
            {
               await Execute<TRequest, TResponse>(handler, request, cancellationToken);
            }

            return default!;
        }
    }
}