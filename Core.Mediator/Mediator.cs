using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Pipelines;

namespace Core.Mediator
{
    /// <summary>
    /// Mediator which uses command and query pipelines for action wrapping and handler execution
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<MediatorResponse<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            var pipelines = GetQueryPipelines<TResponse>();
            static Task<TResponse> Seed() => Task.FromResult<TResponse>(default!);
            var response =  await pipelines
                .Reverse()
                .Aggregate((QueryHandlerDelegate<TResponse>) Seed, (next, pipeline) => () => pipeline.Handle(query, cancellationToken, next))();

            return new MediatorResponse<TResponse>(response);
        }

        private IEnumerable<IQueryPipeline<IQuery<TResponse>, TResponse>> GetQueryPipelines<TResponse>()
        {
            var handlerType = typeof(IQueryPipeline<,>).MakeGenericType(typeof(IQuery<TResponse>), typeof(TResponse));
            var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);
            var pipelines = (IEnumerable<IQueryPipeline<IQuery<TResponse>, TResponse>>) _serviceProvider.GetService(handlerCollectionType);
            foreach (var pipeline in pipelines)
            {
                yield return pipeline;
            }

            yield return new ExecuteHandlerQueryPipeline<IQuery<TResponse>, TResponse>(_serviceProvider);
        }

        public async Task<MediatorResponse> Dispatch<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            var pipelines = GetCommandPipelines<TCommand>();
            static Task Seed() => Task.CompletedTask;
            await pipelines
                .Reverse()
                .Aggregate((CommandHandlerDelegate) Seed, (next, pipeline) => () => pipeline.Handle(command, cancellationToken, next))();

            return new MediatorResponse();
        }

        private IEnumerable<ICommandPipeline<TCommand>> GetCommandPipelines<TCommand>() where TCommand : ICommand
        {
            var handlerType = typeof(ICommandPipeline<>).MakeGenericType(typeof(ICommand));
            var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);
            var pipelines = (IEnumerable<ICommandPipeline<TCommand>>) _serviceProvider.GetService(handlerCollectionType);
            foreach (var pipeline in pipelines)
            {
                yield return pipeline;
            }
            yield return new ExecuteHandlerCommandPipeline<TCommand>(_serviceProvider);
        }
    }
}