using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator;

namespace App.Server.MediatorPipelines
{
    public class ExecuteHandlerQueryPipeline<TRequest, TResponse> : IQueryPipeline<TRequest, TResponse> where TRequest : IQuery<TResponse>
    {
        private readonly IServiceProvider _serviceProvider;

        public ExecuteHandlerQueryPipeline(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, QueryHandlerDelegate<TResponse> next)
        {
            var requestType = request.GetType();
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(requestType, typeof(TResponse));
            var queryHandler = _serviceProvider.GetService(handlerType);
            if (queryHandler == null)
            {
                throw new Exception("No Query handler was found with expected implementation "+handlerType.FullName);
            }

            var method = queryHandler.GetType().GetMethod(nameof(IQueryHandler<IQuery<object>,object>.Handle));
            var task = (Task<TResponse>)method!.Invoke(queryHandler, new object[] {request, cancellationToken})!;
            return await task;
        }
    }
}