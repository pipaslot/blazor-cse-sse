using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using Core.Mediator.CQRSExtensions;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorPipelines
{
    public class QuerySpecificPipeline<TQuery, TResponse> : IPipeline<TQuery, TResponse> where TQuery : IRequest<TResponse>
    {
        private readonly ILogger<Program> _logger;

        public QuerySpecificPipeline(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(TQuery request)
        {
            return request is IQuery;
        }

        public async Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogInformation("Hello from " + nameof(QuerySpecificPipeline<IRequest<object>, object>));

            return await next();
        }
    }

}