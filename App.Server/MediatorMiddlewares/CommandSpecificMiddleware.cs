using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorMiddlewares
{
    public class CommandSpecificMiddleware : IEventMiddleware
    {
        private readonly ILogger<Program> _logger;

        public CommandSpecificMiddleware(ILogger<Program> logger)
        {
            _logger = logger;
        }
        public async Task Handle<TRequest>(TRequest request, CancellationToken cancellationToken, MiddlewareDelegate next) where TRequest : IEvent
        {
            _logger.LogInformation("Hello from " + nameof(CommandSpecificMiddleware));

            await next();
        }
    }
}