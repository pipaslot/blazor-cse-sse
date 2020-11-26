﻿using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Core.Mediator.Abstractions
{
    /// <summary>
    /// Minimal command handler implementation
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task<object?> Handle(TCommand request, CancellationToken cancellationToken)
        {
            await Execute(request, cancellationToken);
            return null;
        }

        protected abstract Task Execute(TCommand request, CancellationToken cancellationToken);
    }
}