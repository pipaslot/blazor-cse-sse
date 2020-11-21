using System;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Commands;
using Core.Mediator;

namespace App.Server.Handlers.Commands
{
    public class ErrorGeneratorCommandHandler : ICommandHandler<ErrorGenerator.Command>
    {
        public Task Handle(ErrorGenerator.Command command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
