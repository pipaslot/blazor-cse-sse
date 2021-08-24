using System.Threading;
using System.Threading.Tasks;
using Pipaslot.Mediator;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging;
using App.Shared.DemoActions;

namespace App.Server.Handlers.DemoActions
{
    public class MultiHandlerMessageHandlerAlwaysLast : IMessageHandler<MultiHandlerMessage.Command>
    {
        private readonly ILogger<MultiHandlerMessageHandlerAlwaysLast> _logger;

        public MultiHandlerMessageHandlerAlwaysLast(ILogger<MultiHandlerMessageHandlerAlwaysLast> logger)
        {
            _logger = logger;
        }

        public int Order => 2;

        public Task Handle(MultiHandlerMessage.Command request, CancellationToken cancellationToken)
        {
            using (_logger.BeginMethod())
            {
                _logger.LogInformation("Handled by handler: Last");
                return Task.CompletedTask;
            }
        }
    }

    public class MultiHandlerCommandHandler1 : IMessageHandler<MultiHandlerMessage.Command>, ISequenceHandler
    {
        private readonly ILogger<MultiHandlerCommandHandler1> _logger;

        public MultiHandlerCommandHandler1(ILogger<MultiHandlerCommandHandler1> logger)
        {
            _logger = logger;
        }

        public int Order => 1;

        public Task Handle(MultiHandlerMessage.Command request, CancellationToken cancellationToken)
        {
            using (_logger.BeginMethod())
            {
                _logger.LogInformation("Handled by handler: " + Order);
                return Task.CompletedTask;
            }
        }
    }

    public class MultiHandlerCommandHandler2 : IMessageHandler<MultiHandlerMessage.Command>, ISequenceHandler
    {
        private readonly ILogger<MultiHandlerCommandHandler2> _logger;

        public MultiHandlerCommandHandler2(ILogger<MultiHandlerCommandHandler2> logger)
        {
            _logger = logger;
        }

        public int Order => 2;

        public Task Handle(MultiHandlerMessage.Command request, CancellationToken cancellationToken)
        {
            using (_logger.BeginMethod())
            {
                _logger.LogInformation("Handled by handler: " + Order);
                return Task.CompletedTask;
            }
        }
    }
}
