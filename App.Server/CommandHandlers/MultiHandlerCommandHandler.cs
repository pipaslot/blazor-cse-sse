using System.Threading;
using System.Threading.Tasks;
using App.Shared.Commands;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.Logging;
using Pipaslot.Logging;

namespace App.Server.CommandHandlers
{
    public class MultiHandlerCommandHandler1 : ICommandHandler<MultiHandler.Command>
    {
        private readonly ILogger<MultiHandlerCommandHandler1> _logger;

        public MultiHandlerCommandHandler1(ILogger<MultiHandlerCommandHandler1> logger)
        {
            _logger = logger;
        }

        public Task<object?> Handle(MultiHandler.Command request, CancellationToken cancellationToken)
        {
            using (_logger.BeginMethod())
            {
                _logger.LogInformation("Handled by handler 1");
                return Task.FromResult<object?>(null);
            }
        }
    }
    public class MultiHandlerCommandHandler2 : ICommandHandler<MultiHandler.Command>
    {
        private readonly ILogger<MultiHandlerCommandHandler2> _logger;

        public MultiHandlerCommandHandler2(ILogger<MultiHandlerCommandHandler2> logger)
        {
            _logger = logger;
        }

        public Task<object?> Handle(MultiHandler.Command request, CancellationToken cancellationToken)
        {
            using (_logger.BeginMethod())
            {
                _logger.LogInformation("Handled by handler 2");
                return Task.FromResult<object?>(null);
            }
        }
    }
}
