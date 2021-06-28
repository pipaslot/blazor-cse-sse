using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorPipelines
{
    public class CommandSpecificPipeline : IEventPipeline
    {
        private readonly ILogger<Program> _logger;

        public CommandSpecificPipeline(ILogger<Program> logger)
        {
            _logger = logger;
        }
        public async Task Handle<TRequest>(TRequest request, CancellationToken cancellationToken, EventHandlerDelegate next) where TRequest : IEvent
        {
            _logger.LogInformation("Hello from " + nameof(CommandSpecificPipeline));

            await next();
        }
    }
}