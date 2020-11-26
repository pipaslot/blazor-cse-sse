using System;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Commands;
using Core.Mediator.Abstractions;

namespace App.Server.CommandHandlers
{
    // ReSharper disable once UnusedMember.Global
    public class ErrorGeneratorCommandHandler : ICommandHandler<ErrorGenerator.Command>
    {
        public Task<object> Handle(ErrorGenerator.Command command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
