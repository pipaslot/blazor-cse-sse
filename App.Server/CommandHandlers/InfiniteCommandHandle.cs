using System.Threading;
using System.Threading.Tasks;
using App.Shared.Commands;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;

namespace App.Server.CommandHandlers
{
    public class InfiniteCommandHandler : ICommandHandler<Infinite.Command>
    {
        private readonly ILogger<InfiniteCommandHandler> _logger;

        public InfiniteCommandHandler(ILogger<InfiniteCommandHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(Infinite.Command request, CancellationToken cancellationToken)
        {
            while (true)
            {
                _logger.LogInformation("Infinite action");
                Thread.Sleep(1000);
                if (cancellationToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }
            }
        }
    }
}
