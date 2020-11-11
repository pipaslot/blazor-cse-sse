using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Shared.Mediator;

namespace App.Server.Services
{
    public class ServerMediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public ServerMediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<MediatorResponse<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IQueryPipeline<,>).MakeGenericType(typeof(IQuery<TResponse>), typeof(TResponse));
            var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);
            var pipelines = (IEnumerable<IQueryPipeline<IQuery<TResponse>, TResponse>>) _serviceProvider.GetService(handlerCollectionType);
            static Task<TResponse> Seed() => Task.FromResult<TResponse>(default!);
            var response =  await pipelines
                .Reverse()
                .Aggregate((QueryHandlerDelegate<TResponse>) Seed, (next, pipeline) => () => pipeline.Handle(query, cancellationToken, next))();

            return new MediatorResponse<TResponse>(response);
        }

        public async Task<MediatorResponse> Publish<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            var handlerType = typeof(ICommandPipeline<>).MakeGenericType(typeof(ICommand));
            var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);
            var pipelines = (IEnumerable<ICommandPipeline<ICommand>>) _serviceProvider.GetService(handlerCollectionType);
            static Task Seed() => Task.CompletedTask;
            await pipelines
                .Reverse()
                .Aggregate((QueryHandlerDelegate) Seed, (next, pipeline) => () => pipeline.Handle(command, cancellationToken, next))();

            return new MediatorResponse();
        }
    }
}