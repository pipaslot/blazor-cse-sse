using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Mediator.Abstractions;

namespace Core.Mediator
{
    /// <summary>
    /// Mediator which wrapps handler execution into pipelines
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly ServiceResolver _handlerResolver;

        public Mediator(ServiceResolver handlerResolver)
        {
            _handlerResolver = handlerResolver;
        }

        public async Task<MediatorResponse> Fire(IEvent @event, CancellationToken cancellationToken = default)
        {
            var pipeline = _handlerResolver.GetEventPipeline(@event.GetType());
            static Task Seed() => Task.CompletedTask;
            try
            {
                await pipeline
                    .Reverse()
                    .Aggregate((MiddlewareDelegate)Seed,
                        (next, middleware) => () => middleware.Handle(@event, cancellationToken, next))();

                return new MediatorResponse();
            }
            catch (Exception e)
            {
                return new MediatorResponse(e.Message);
            }
        }

        public async Task<MediatorResponse<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var pipeline = _handlerResolver.GetRequestPipeline(request.GetType());
            static Task<TResponse> Seed() => Task.FromResult<TResponse>(default!);
            try
            {
                var response = await pipeline
                    .Reverse()
                    .Aggregate((MiddlewareDelegate<TResponse>)Seed,
                        (next, middleware) => () => middleware.Handle(request, cancellationToken, next))();

                return new MediatorResponse<TResponse>(response);
            }
            catch (Exception e)
            {
                return new MediatorResponse<TResponse>(e.Message);
            }
        }
    }
}