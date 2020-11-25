using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;

namespace Core.Mediator.Pipelines
{
    /// <summary>
    /// This pipeline must be always registered as the last one, because it is executing query handler
    /// </summary>
    internal class ExecuteHandlerPipeline<TRequest, TResponse> : IPipeline<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IServiceProvider _serviceProvider;

        public ExecuteHandlerPipeline(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool CanHandle(TRequest request)
        {
            return true;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var queryType = request.GetType();
            var handlerType = typeof(IHandler<,>).MakeGenericType(queryType, typeof(TResponse));
            var queryHandler = _serviceProvider.GetService(handlerType);
            if (queryHandler == null)
            {
                throw new Exception("No Query handler was found with expected implementation " + handlerType.FullName);
            }

            var method = queryHandler.GetType().GetMethod(nameof(IHandler<IRequest<object>, object>.Handle));
            try
            {
                var task = (Task<TResponse>)method!.Invoke(queryHandler, new object[] { request, cancellationToken })!;
                return await task;
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }

                throw;
            }
        }
    }
}