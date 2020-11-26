using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorPipelines
{
    public class CommandSpecificPipeline : IPipeline
    {
        private readonly ILogger<Program> _logger;

        public CommandSpecificPipeline(ILogger<Program> logger)
        {
            _logger = logger;
        }
        public async Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) where TRequest : IRequest<TResponse>
        {
            _logger.LogInformation("Hello from " + nameof(CommandSpecificPipeline));

            return await next();
        }
    }
}