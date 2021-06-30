using Core.Mediator.Abstractions;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;
using Core.Mediator.Middlewares;

namespace Core.Mediator
{
    public class ServiceResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        /// <summary>
        /// Get all registered handlers from service provider
        /// </summary>
        public object[] GetEventHandlers(Type eventType)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            return _serviceProvider.GetServices(handlerType)
                .Where(h => h != null)
                // ReSharper disable once RedundantEnumerableCastCall
                .Cast<object>()
                .ToArray();
        }

        /// <summary>
        /// Get all registered handlers from service provider
        /// </summary>
        public object[] GetRequestHandlers<TResponse>(Type requestType)
        {
            return GetRequestHandlers(requestType, typeof(TResponse));
        }


        /// <summary>
        /// Get all registered handlers from service provider
        /// </summary>
        public object[] GetRequestHandlers(Type requestType, Type responseType)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
            return _serviceProvider.GetServices(handlerType)
                .Where(h => h != null)
                // ReSharper disable once RedundantEnumerableCastCall
                .Cast<object>()
                .ToArray();
        }

        public IExecutiveMiddleware GetRequestExecutiveMiddleware(Type requestType)
        {
            var pipeline = GetRequestPipeline(requestType).Last();
            if (pipeline is IExecutiveMiddleware ep)
            {
                return ep;
            }
            throw new Exception("Executive pipeline not found");//This should never happen as GetRequestPipelines always returns last pipeline as executive
        }

        public IEnumerable<IRequestMiddleware> GetRequestPipeline(Type requestType)
        {
            var pipelines = GetPipeline<IRequestMiddleware>(requestType);

            foreach (var pipeline in pipelines)
            {
                yield return pipeline;
                if (pipeline is IExecutiveMiddleware)
                {
                    yield break;
                }
            }

            yield return new SingleHandlerExecutionRequestMiddleware(this);
        }

        public IExecutiveMiddleware GetEventExecutiveMiddleware(Type requestType)
        {
            var pipeline = GetEventPipeline(requestType).Last();
            if (pipeline is IExecutiveMiddleware ep)
            {
                return ep;
            }
            throw new Exception("Executive pipeline not found");//This should never happen as GetEventPipelines always returns last pipeline as executive
        }

        public IEnumerable<IEventMiddleware> GetEventPipeline(Type requestType)
        {
            var pipelines = GetPipeline<IEventMiddleware>(requestType);

            foreach (var pipeline in pipelines)
            {
                yield return pipeline;
                if (pipeline is IExecutiveMiddleware)
                {
                    yield break;
                }
            }

            yield return new SingleHandlerExecutionEventMiddleware(this);
        }

        private IEnumerable<TItem> GetPipeline<TItem>(Type requestType)
        {
            return _serviceProvider.GetServices<PipelineDefinition>()
                .ToArray()
                .Where(d => d.MarkerType == null || d.MarkerType.IsAssignableFrom(requestType))
                .Select(d => (TItem)_serviceProvider.GetRequiredService(d.PipelineType));
        }
    }
}
