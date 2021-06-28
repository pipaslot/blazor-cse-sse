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
        private readonly ServiceResolver _handlerResolver;

        public Mediator(IServiceProvider serviceProvider, ServiceResolver handlerResolver)
        {
            _serviceProvider = serviceProvider;
            _handlerResolver = handlerResolver;
        }

        public async Task<MediatorResponse> Fire(IEvent @event, CancellationToken cancellationToken = default)
        {
            var pipelines = _handlerResolver.GetEventPipelines(@event.GetType());
            static Task Seed() => Task.CompletedTask;
            try
            {
                await pipelines
                    .Reverse()
                    .Aggregate((EventHandlerDelegate)Seed,
                        (next, pipeline) => () => pipeline.Handle(@event, cancellationToken, next))();

                return new MediatorResponse();
            }
            catch (Exception e)
            {
                return new MediatorResponse(e.Message);
            }
        }

        public async Task<MediatorResponse<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var pipelines = _handlerResolver.GetRequestPipelines(request.GetType());
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
    }
}