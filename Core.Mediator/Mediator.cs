﻿using System;
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

        public async Task<IMediatorResponse> Fire(IEvent @event, CancellationToken cancellationToken = default)
        {
            var pipeline = _handlerResolver.GetPipeline(@event.GetType());
            static Task Seed() => Task.CompletedTask;
            var response = new MediatorResponse();
            try
            {
                await pipeline
                    .Reverse()
                    .Aggregate((MiddlewareDelegate)Seed,
                        (next, middleware) => () => middleware.Invoke(@event, response, next, cancellationToken))();

                return new MediatorResponse();
            }
            catch (Exception e)
            {
                return new MediatorResponse(e.Message);
            }
        }

        public async Task<IMediatorResponse<TResponse>> Execute<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var pipeline = _handlerResolver.GetPipeline(request.GetType());
            static Task Seed() => Task.CompletedTask;
            var response = new MediatorResponse<TResponse>();
            try
            {
                await pipeline
                    .Reverse()
                    .Aggregate((MiddlewareDelegate)Seed,
                        (next, middleware) => () => middleware.Invoke(request, response, next, cancellationToken))();

                return response;
            }
            catch (Exception e)
            {
                return new MediatorResponse<TResponse>(e.Message);
            }
        }
    }
}