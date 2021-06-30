using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;

namespace Core.Mediator.Middlewares
{
    public abstract class ExecutionMiddleware : IMiddleware, IExecutionMiddleware
    {
        public abstract bool ExecuteMultipleHandlers { get; }
        protected abstract Task HandleEvent<TEvent>(TEvent @event, CancellationToken cancellationToken);
        protected abstract Task HandleRequest<TRequest>(TRequest request, MediatorResponse response, CancellationToken cancellationToken);

        public async Task Invoke<TAction>(TAction action, MediatorResponse response, MiddlewareDelegate next, CancellationToken cancellationToken)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));

            }
            if (action is IEvent e)
            {
                await HandleEvent(e, cancellationToken);
            }
            else
            {
                await HandleRequest(action, response, cancellationToken);
            }
        }

        /// <summary>
        /// Execute handler
        /// </summary>
        protected async Task ExecuteEvent<TEvent>(object handler, TEvent @event, CancellationToken cancellationToken)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));
            var method = handler.GetType().GetMethod(nameof(IRequestHandler<IRequest<object>, object>.Handle));
            try
            {
                await OnBeforeHandlerExecution(handler, @event);
                var task = (Task?)method!.Invoke(handler, new object[] { @event, cancellationToken })!;
                if (task != null)
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
        /// Execute handler
        /// </summary>
        protected async Task ExecuteRequest<TRequest>(object handler, TRequest request, MediatorResponse response, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var method = handler.GetType().GetMethod(nameof(IRequestHandler<IRequest<object>, object>.Handle));
            try
            {
                await OnBeforeHandlerExecution(handler, request);
                var task = (Task?)method!.Invoke(handler, new object[] { request, cancellationToken })!;
                if (task != null)
                {
                    await task.ConfigureAwait(false);

                    var resultProperty = task.GetType().GetProperty("Result");
                    var result = resultProperty?.GetValue(task);
                    if (result != null)
                    {
                        await OnSuccessExecution(handler, request);
                        response.Results.Add(result);
                    }
                }
            }
            catch (TargetInvocationException e)
            {
                await OnFailedExecution(handler, request, e.InnerException ?? e);
                if (e.InnerException != null)
                {
                    // Unwrap exception
                    throw e.InnerException;
                }

                throw;
            }
            finally
            {
                await OnAfterHandlerExecution(handler, request);
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

    }
}
