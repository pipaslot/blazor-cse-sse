using System;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.DemoActions;
using Pipaslot.Mediator.Abstractions;

namespace App.Server.Handlers.DemoActions
{
    // ReSharper disable once UnusedMember.Global
    public class ValidationErrorGeneratorCommandHandler : IMessageHandler<ValidationErrorGeneratorMessage.Command>
    {
        public Task Handle(ValidationErrorGeneratorMessage.Command command, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
