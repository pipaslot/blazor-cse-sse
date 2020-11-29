using System;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Commands;
using Core.Mediator.Abstractions;

namespace App.Server.CommandHandlers
{
    // ReSharper disable once UnusedMember.Global
    public class ValidationErrorGeneratorCommandHandler : ICommandHandler<ValidationErrorGenerator.Command>
    {
        public Task<object?> Handle(ValidationErrorGenerator.Command command, CancellationToken cancellationToken)
        {
            return Task.FromResult<object?>(null);
        }
    }
}
