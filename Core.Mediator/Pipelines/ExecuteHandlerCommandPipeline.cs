using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;

namespace Core.Mediator.Pipelines
{
    /// <summary>
    /// This pipeline must be always registered as the last one, because it is executing command handler
    /// </summary>
    internal class ExecuteHandlerCommandPipeline<TRequest, TResponse> : IPipeline<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IServiceProvider _serviceProvider;

        public ExecuteHandlerCommandPipeline(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool CanHandle(TRequest request)
        {
            return request is ICommand;
        }
        
        public async Task<TResponse> Handle(TRequest command, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            var commandHandler = _serviceProvider.GetService(handlerType);
            if (commandHandler == null)
            {
                throw new Exception("No Command handler was found for "+(typeof(TRequest).FullName));
            }
            
            var method = commandHandler.GetType().GetMethod(nameof(ICommandHandler<ICommand>.Handle));
            try
            {
                var task = (Task<TResponse>)method!.Invoke(commandHandler, new object[] { command, cancellationToken })!;
                return await task;
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }

                throw;
            }
        }
    }
}