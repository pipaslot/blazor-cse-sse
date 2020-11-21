using System;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Command;
using Core.Mediator;

namespace App.Server.CommandHandlers
{
    public class ErrorGeneratorCommandHandler : ICommandHandler<ErrorGenerator.Command>
    {
        public Task Handle(ErrorGenerator.Command command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
