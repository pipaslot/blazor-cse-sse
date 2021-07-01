using System;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Commands;
using App.Shared.CQRSAbstraction;

namespace App.Server.CommandHandlers
{
    // ReSharper disable once UnusedMember.Global
    public class ValidationErrorGeneratorCommandHandler : ICommandHandler<ValidationErrorGenerator.Command>
    {
        public Task Handle(ValidationErrorGenerator.Command command, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
