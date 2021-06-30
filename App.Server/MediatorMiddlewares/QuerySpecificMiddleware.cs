using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorMiddlewares
{
    public class QuerySpecificMiddleware : IRequestMiddleware
    {
        private readonly ILogger<Program> _logger;

        public QuerySpecificMiddleware(ILogger<Program> logger)
        {
            _logger = logger;
        }


        public async Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, MiddlewareDelegate<TResponse> next) where TRequest : IRequest<TResponse>
        {
            _logger.LogInformation("Hello from " + nameof(QuerySpecificMiddleware));

            return await next();
        }
    }

}