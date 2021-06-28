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
    public class SingleHandlerExecutionEventPipeline : BaseEventPipeline
    {
        public SingleHandlerExecutionEventPipeline(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override async Task Handle<TEvent>(TEvent @event, CancellationToken cancellationToken, EventHandlerDelegate next)
        {
            var handlers = GetRegisteredHandlers<TEvent>(@event);
            if (handlers.Length > 1)
            {
                throw new Exception($"Multiple handlers were registered for the same request. Remove one from defined type: {string.Join(" OR ", handlers)}");
            }

            var handler = handlers.FirstOrDefault();
            if (handler == null)
            {
                throw new Exception("No handler was found for " + @event.GetType());
            }
            await Execute<TEvent>(handler, @event, cancellationToken);
        }
    }
}