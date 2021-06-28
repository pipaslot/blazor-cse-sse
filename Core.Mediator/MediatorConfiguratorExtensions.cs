using Core.Mediator.Abstractions;
using Core.Mediator.Pipelines;

namespace Core.Mediator
{
    public static class MediatorConfiguratorExtensions
    {
        /// <summary>
        /// Pipeline running all handlers concurrently. Not further pipeline will be executed after this one for specified Action Marker.
        /// </summary>
        /// <typeparam name="TActionMarker">Action interface</typeparam>
        public static MediatorConfigurator UseEventOnlyConcurrentMultiHandler<TActionMarker>(this MediatorConfigurator config)
            where TActionMarker : IEvent
        {
            return config.UseEventOnly<MultiHandlerConcurrentExecutionEventPipeline, TActionMarker>();
        }

        /// <summary>
        /// Pipeline running all handlers concurrently. Not further pipeline will be executed after this one for specified Action Marker.
        /// </summary>
        /// <typeparam name="TActionMarker">Action interface</typeparam>
        public static MediatorConfigurator UseRequestOnlyConcurrentMultiHandler<TActionMarker>(this MediatorConfigurator config)
            where TActionMarker : IRequest
        {
            return config.UseRequestOnly<MultiHandlerConcurrentExecutionRequestPipeline, TActionMarker>();
        }

        /// <summary>
        /// Pipeline running all handlers in sequence one by one. Not further pipeline will be executed after this one for specified Action Marker.
        /// </summary>
        /// <typeparam name="TActionMarker">Action interface</typeparam>
        public static MediatorConfigurator UseEventOnlySequenceMultiHandler<TActionMarker>(this MediatorConfigurator config)
            where TActionMarker : IEvent
        {
            return config.UseEventOnly<MultiHandlerSequenceExecutionEventPipeline, TActionMarker>();
        }

        /// <summary>
        /// Pipeline running all handlers in sequence one by one. Not further pipeline will be executed after this one for specified Action Marker.
        /// </summary>
        /// <typeparam name="TActionMarker">Action interface</typeparam>
        public static MediatorConfigurator UseRequestOnlySequenceMultiHandler<TActionMarker>(this MediatorConfigurator config)
            where TActionMarker : IRequest
        {
            return config.UseRequestOnly<MultiHandlerSequenceExecutionRequestPipeline, TActionMarker>();
        }
    }
}
