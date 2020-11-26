using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;
using Core.Mediator.Pipelines;
using Microsoft.Extensions.DependencyInjection;

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

        public async Task<MediatorResponse<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var pipelines = GetPipelines(request.GetType());
            static Task<TResponse> Seed() => Task.FromResult<TResponse>(default!);
            try
            {
                var response = await pipelines
                    .Reverse()
                    .Aggregate((RequestHandlerDelegate<TResponse>)Seed,
                        (next, pipeline) => () => pipeline.Handle(request, cancellationToken, next))();

                return new MediatorResponse<TResponse>(response);
            }
            catch (Exception e)
            {
                return new MediatorResponse<TResponse>(e.Message);
            }
        }

        private IEnumerable<IPipeline> GetPipelines(Type requestType)
        {
            var pipelines = _serviceProvider.GetServices<PipelineDefinition>()
                .ToArray()
                .Where(d => d.MarkerType == null || d.MarkerType.IsAssignableFrom(requestType))
                .Select(d=>(IPipeline)_serviceProvider.GetRequiredService(d.PipelineType));
           
            foreach (var pipeline in pipelines)
            {
                yield return pipeline;
            }

            yield return new SingleHandlerExecutionPipeline<IRequest>(_serviceProvider);
        }
    }
}