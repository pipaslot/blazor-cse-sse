using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator.Pipelines
{
    /// <summary>
    /// This pipeline must be always registered as the last one, because it is executing query handler
    /// </summary>
    internal class ExecuteHandlerQueryPipeline<TQuery, TResponse> : IQueryPipeline<TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        private readonly IServiceProvider _serviceProvider;

        public ExecuteHandlerQueryPipeline(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken, QueryHandlerDelegate<TResponse> next)
        {
            var queryType = query.GetType();
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));
            var queryHandler = _serviceProvider.GetService(handlerType);
            if (queryHandler == null)
            {
                throw new Exception("No Query handler was found with expected implementation "+handlerType.FullName);
            }

            var method = queryHandler.GetType().GetMethod(nameof(IQueryHandler<IQuery<object>,object>.Handle));
            var task = (Task<TResponse>)method!.Invoke(queryHandler, new object[] {query, cancellationToken})!;
            return await task;
        }
    }
}