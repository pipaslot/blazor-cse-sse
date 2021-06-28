using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Mediator.Pipelines
{
    public abstract class BaseEventPipeline : IEventPipeline
    {
        private readonly ServiceResolver _handlerResolver;

        protected BaseEventPipeline(ServiceResolver handlerResolver)
        {
            _handlerResolver = handlerResolver;
        }
        /// <summary>
        /// Get all registered handlers from service provider
        /// </summary>
        protected object[] GetRegisteredHandlers<TEvent>(TEvent request)
        {
            if (request == null)
            {
                return new object[0];
            }
            return _handlerResolver.GetEventHandlers(request.GetType());
        }

        /// <summary>
        /// Execute handler
        /// </summary>
        protected async Task Execute<TEvent>(object handler, TEvent @event, CancellationToken cancellationToken)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));
            var method = handler.GetType().GetMethod(nameof(IRequestHandler<IRequest<object>, object>.Handle));
            try
            {
                await OnBeforeHandlerExecution(handler, @event);
                var task = (Task?)method!.Invoke(handler, new object[] { @event, cancellationToken })!;
                if(task != null)
                {
                    await task;
                }
                await OnSuccessExecution(handler, @event);

            }
            catch (TargetInvocationException e)
            {
                await OnFailedExecution(handler, @event, e.InnerException ?? e);
                if (e.InnerException != null)
                {
                    // Unwrap exception
                    throw e.InnerException;
                }

                throw;
            }
            finally
            {
                await OnAfterHandlerExecution(handler, @event);
            }
        }

        /// <summary>
        /// Hook method called always before handler execution
        /// </summary>
        protected virtual Task OnBeforeHandlerExecution<TEvent>(object handler, TEvent request)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook method called always after handler execution
        /// </summary>
        protected virtual Task OnAfterHandlerExecution<TEvent>(object handler, TEvent request)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook called only after successful handler execution. Is omitted if exception is thrown
        /// </summary>
        /// <param name="handler">Request handler</param>
        /// <param name="request">Handler input data</param>
        /// <returns></returns>
        protected virtual Task OnSuccessExecution<TEvent>(object handler, TEvent request)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook called only if exception is thrown during handler execution
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="request"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual Task OnFailedExecution<TEvent>(object handler, TEvent request, Exception e)
        {
            return Task.CompletedTask;
        }

        public abstract Task Handle<TEvent>(TEvent request,
            CancellationToken cancellationToken, EventHandlerDelegate next)
            where TEvent : IEvent;
    }
}
