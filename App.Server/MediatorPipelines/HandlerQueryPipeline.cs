using System;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Mediator;

namespace App.Server.MediatorPipelines
{
    public class HandlerQueryPipeline<TRequest, TResponse> : IQueryPipeline<TRequest, TResponse> where TRequest : IQuery<TResponse>
    {
        private readonly IServiceProvider _serviceProvider;

        public HandlerQueryPipeline(IServiceProvider serviceProvider)
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

            var method = queryHandler.GetType().GetMethod("Handle");
            var task = (Task<TResponse>)method.Invoke(queryHandler, new object[] {request, cancellationToken});
            return await task;
            // var response = await queryHandler.Handle(request, cancellationToken);
            // return response;
        }
    }
}