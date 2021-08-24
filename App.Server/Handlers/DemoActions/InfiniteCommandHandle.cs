using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Mediator.Abstractions;
using Microsoft.Extensions.Logging;
using App.Shared.DemoActions;

namespace App.Server.Handlers.DemoActions
{
    public class InfiniteMessageHandler : IMessageHandler<InfiniteMessage.Command>
    {
        private readonly ILogger<InfiniteMessageHandler> _logger;

        public InfiniteMessageHandler(ILogger<InfiniteMessageHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(InfiniteMessage.Command request, CancellationToken cancellationToken)
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
