using System;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Mediator;

namespace App.Server.MediatorPipelines
{
    public class HandlerCommandPipeline<TRequest> : ICommandPipeline<TRequest> where TRequest : ICommand
    {
        private readonly IServiceProvider _serviceProvider;

        public HandlerCommandPipeline(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(TRequest request, CancellationToken cancellationToken, QueryHandlerDelegate next)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(typeof(TRequest));
            var commandHandler = (ICommandHandler<TRequest>) _serviceProvider.GetService(handlerType);
            if (commandHandler == null)
            {
                throw new Exception("No Command handler was found for "+(typeof(TRequest).FullName));
            }
            await commandHandler.Handle(request, cancellationToken);
        }
    }
}