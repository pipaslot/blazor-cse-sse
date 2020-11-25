using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
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

        public async Task<MediatorResponse<TResponse>> Send<TResponse>(IRequest<TResponse> query, CancellationToken cancellationToken = default)
        {
            var pipelines = GetPipelines<TResponse>()
                .Where(p => p.CanHandle(query));
            static Task<TResponse> Seed() => Task.FromResult<TResponse>(default!);
            try
            {
                var response = await pipelines
                    .Reverse()
                    .Aggregate((RequestHandlerDelegate<TResponse>)Seed,
                        (next, pipeline) => () => pipeline.Handle(query, cancellationToken, next))();

                return new MediatorResponse<TResponse>(response);
            }
            catch (Exception e)
            {
                return new MediatorResponse<TResponse>(e.Message);
            }
        }

        private IEnumerable<IPipeline<IRequest<TResponse>, TResponse>> GetPipelines<TResponse>()
        {
            var handlerType = typeof(IPipeline<,>).MakeGenericType(typeof(IRequest<TResponse>), typeof(TResponse));
            var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);
            var pipelines = (IEnumerable<IPipeline<IRequest<TResponse>, TResponse>>)_serviceProvider.GetService(handlerCollectionType);
            foreach (var pipeline in pipelines)
            {
                yield return pipeline;
            }

            yield return new ExecuteHandlerCommandPipeline<IRequest<TResponse>, TResponse>(_serviceProvider);
            yield return new ExecuteHandlerPipeline<IRequest<TResponse>, TResponse>(_serviceProvider);
        }
    }
}