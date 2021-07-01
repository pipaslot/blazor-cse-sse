using System.Threading;
using System.Threading.Tasks;
using App.Shared.Commands;
using App.Shared.CQRSAbstraction;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging;

namespace App.Server.CommandHandlers
{
    public class MultiHandlerCommandHandlerAlwaysLast : ICommandHandler<MultiHandler.Command>
    {
        private readonly ILogger<MultiHandlerCommandHandlerAlwaysLast> _logger;

        public MultiHandlerCommandHandlerAlwaysLast(ILogger<MultiHandlerCommandHandlerAlwaysLast> logger)
        {
            _logger = logger;
        }

        public int Order => 2;

        public Task Handle(MultiHandler.Command request, CancellationToken cancellationToken)
        {
            using (_logger.BeginMethod())
            {
                _logger.LogInformation("Handled by handler: Last");
                return Task.CompletedTask;
            }
        }
    }

    public class MultiHandlerCommandHandler1 : ICommandHandler<MultiHandler.Command>, ISequenceHandler
    {
        private readonly ILogger<MultiHandlerCommandHandler1> _logger;

        public MultiHandlerCommandHandler1(ILogger<MultiHandlerCommandHandler1> logger)
        {
            _logger = logger;
        }

        public int Order => 1;

        public Task Handle(MultiHandler.Command request, CancellationToken cancellationToken)
        {
            using (_logger.BeginMethod())
            {
                _logger.LogInformation("Handled by handler: " + Order);
                return Task.CompletedTask;
            }
        }
    }

    public class MultiHandlerCommandHandler2 : ICommandHandler<MultiHandler.Command>, ISequenceHandler
    {
        private readonly ILogger<MultiHandlerCommandHandler2> _logger;

        public MultiHandlerCommandHandler2(ILogger<MultiHandlerCommandHandler2> logger)
        {
            _logger = logger;
        }

        public int Order => 2;

        public Task Handle(MultiHandler.Command request, CancellationToken cancellationToken)
        {
            using (_logger.BeginMethod())
            {
                _logger.LogInformation("Handled by handler: " + Order);
                return Task.CompletedTask;
            }
        }
    }
}
