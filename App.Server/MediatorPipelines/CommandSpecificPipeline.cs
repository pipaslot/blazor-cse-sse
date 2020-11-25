using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;

namespace App.Server.MediatorPipelines
{
    public class CommandSpecificPipeline<TQuery, TResponse> : IPipeline<TQuery, TResponse> where TQuery : IRequest<TResponse>
    {
        private readonly ILogger<Program> _logger;

        public CommandSpecificPipeline(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(TQuery request)
        {
            return request is ICommand;
        }

        public async Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogInformation("Hello from " + nameof(CommandSpecificPipeline<IRequest<object>, object>));

            return await next();
        }
    }

}