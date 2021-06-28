using Core.Mediator.Abstractions;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;
using Core.Mediator.Pipelines;

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

        public IExecutivePipeline GetRequestExecutivePipeline(Type requestType)
        {
            var pipeline = GetRequestPipelines(requestType).Last();
            if (pipeline is IExecutivePipeline ep)
            {
                return ep;
            }
            throw new Exception("Executive pipeline not found");//This should never happen as GetRequestPipelines always returns last pipeline as executive
        }

        public IEnumerable<IRequestPipeline> GetRequestPipelines(Type requestType)
        {
            var pipelines = GetPipelines<IRequestPipeline>(requestType);

            foreach (var pipeline in pipelines)
            {
                yield return pipeline;
                if (pipeline is IExecutivePipeline)
                {
                    yield break;
                }
            }

            yield return new SingleHandlerExecutionRequestPipeline(this);
        }

        public IExecutivePipeline GetEventExecutivePipeline(Type requestType)
        {
            var pipeline = GetEventPipelines(requestType).Last();
            if (pipeline is IExecutivePipeline ep)
            {
                return ep;
            }
            throw new Exception("Executive pipeline not found");//This should never happen as GetEventPipelines always returns last pipeline as executive
        }

        public IEnumerable<IEventPipeline> GetEventPipelines(Type requestType)
        {
            var pipelines = GetPipelines<IEventPipeline>(requestType);

            foreach (var pipeline in pipelines)
            {
                yield return pipeline;
                if (pipeline is IExecutivePipeline)
                {
                    yield break;
                }
            }

            yield return new SingleHandlerExecutionEventPipeline(this);
        }

        private IEnumerable<TItem> GetPipelines<TItem>(Type requestType)
        {
            return _serviceProvider.GetServices<PipelineDefinition>()
                .ToArray()
                .Where(d => d.MarkerType == null || d.MarkerType.IsAssignableFrom(requestType))
                .Select(d => (TItem)_serviceProvider.GetRequiredService(d.PipelineType));
        }
    }
}
