using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Mediator.Middlewares;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorMiddlewares
{
    public class CommandSpecificMiddleware : IMediatorMiddleware
    {
        private readonly ILogger<Program> _logger;

        public CommandSpecificMiddleware(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public async Task Invoke<TAction>(TAction action, MediatorContext context, MiddlewareDelegate next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hello from " + nameof(CommandSpecificMiddleware));

            await next(context);
        }
    }
}