using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Mediator.Pipelines
{
    /// <summary>
    /// Pipeline executing one handler for request implementing TMarker type
    /// </summary>
    public class SingleHandlerExecutionPipeline<TMarker> : IPipeline
    {
        private readonly IServiceProvider _serviceProvider;

        public SingleHandlerExecutionPipeline(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public virtual bool CanHandle<TRequest>(TRequest request) where TRequest : IRequest
        {
            return request is TMarker;
        }

        public virtual async Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) where TRequest : IRequest<TResponse>
        {
            var handlers = GetRegisteredHandlers<TRequest, TResponse>(request);
            if (handlers.Length == 0)
            {
                throw new Exception("No handler was found for " + request.GetType());
            }
            if (handlers.Length > 1)
            {
                throw new Exception($"Multiple handlers were registered for the same request. Remove one from defined type: {string.Join(" OR ", handlers)}");
            }

            var queryHandler = handlers.First();
            return await Execute<TRequest, TResponse>(queryHandler, request, cancellationToken);
        }

        protected object[] GetRegisteredHandlers<TRequest, TResponse>(TRequest request)
        {
            var queryType = request.GetType();
            var handlerType = typeof(IHandler<,>).MakeGenericType(queryType, typeof(TResponse));
            return _serviceProvider.GetServices(handlerType).ToArray();
        }

        protected async Task<TResponse> Execute<TRequest, TResponse>(object handler, TRequest request, CancellationToken cancellationToken)
        {
            var method = handler.GetType().GetMethod(nameof(IHandler<IRequest<object>, object>.Handle));
            try
            {
                var task = (Task<TResponse>)method!.Invoke(handler, new object[] { request, cancellationToken })!;
                return await task;
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                {
                    // Unwrap exception
                    throw e.InnerException;
                }

                throw;
            }
        }
    }
}