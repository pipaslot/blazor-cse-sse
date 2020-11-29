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
    public class SingleHandlerExecutionPipeline : IPipeline
    {
        private readonly IServiceProvider _serviceProvider;

        public SingleHandlerExecutionPipeline(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public virtual async Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) where TRequest : IRequest<TResponse>
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

        protected object[] GetRegisteredHandlers<TRequest, TResponse>(TRequest request)
        {
            if (request == null)
            {
                return new object[0];
            }
            var queryType = request.GetType();
            var handlerType = typeof(IHandler<,>).MakeGenericType(queryType, typeof(TResponse));
            return _serviceProvider.GetServices(handlerType)
                .Where(h => h != null)
                // ReSharper disable once RedundantEnumerableCastCall
                .Cast<object>()
                .ToArray();
        }

        protected async Task<TResponse> Execute<TRequest, TResponse>(object handler, TRequest request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var method = handler.GetType().GetMethod(nameof(IHandler<IRequest<object>, object>.Handle));
            try
            {
                var task = (Task<TResponse>?)method!.Invoke(handler, new object[] { request, cancellationToken })!;
                if(task != null){
                    return await task;
                }

                return default!;
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