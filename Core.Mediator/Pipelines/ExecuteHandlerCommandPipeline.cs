using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Mediator.Pipelines
{
    /// <summary>
    /// This pipeline must be always registered as the last one, because it is executing command handler
    /// </summary>
    internal class ExecuteHandlerCommandPipeline<TCommand> : ICommandPipeline<TCommand> where TCommand : ICommand
    {
        private readonly IServiceProvider _serviceProvider;

        public ExecuteHandlerCommandPipeline(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(TCommand command, CancellationToken cancellationToken, CommandHandlerDelegate next)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            var commandHandler = _serviceProvider.GetService(handlerType);
            if (commandHandler == null)
            {
                throw new Exception("No Command handler was found for "+(typeof(TCommand).FullName));
            }
            
            var method = commandHandler.GetType().GetMethod(nameof(ICommandHandler<ICommand>.Handle));
            await (Task)method!.Invoke(commandHandler, new object[] {command, cancellationToken})!;
        }
    }
}