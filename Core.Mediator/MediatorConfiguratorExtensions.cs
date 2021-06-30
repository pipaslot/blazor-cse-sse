using Core.Mediator.Abstractions;
using Core.Mediator.Middlewares;

namespace Core.Mediator
{
    public static class MediatorConfiguratorExtensions
    {
        /// <summary>
        /// Pipeline running all handlers concurrently. Not further pipeline will be executed after this one for specified Action Marker.
        /// </summary>
        /// <typeparam name="TActionMarker">Action interface</typeparam>
        public static PipelineConfigurator UseEventOnlyConcurrentMultiHandler<TActionMarker>(this PipelineConfigurator config)
            where TActionMarker : IEvent
        {
            return config.UseEventOnly<MultiHandlerConcurrentExecutionEventMiddleware, TActionMarker>();
        }

        /// <summary>
        /// Pipeline running all handlers concurrently. Not further pipeline will be executed after this one for specified Action Marker.
        /// </summary>
        /// <typeparam name="TActionMarker">Action interface</typeparam>
        public static PipelineConfigurator UseRequestOnlyConcurrentMultiHandler<TActionMarker>(this PipelineConfigurator config)
            where TActionMarker : IRequest
        {
            return config.UseRequestOnly<MultiHandlerConcurrentExecutionRequestMiddleware, TActionMarker>();
        }

        /// <summary>
        /// Pipeline running all handlers in sequence one by one. Not further pipeline will be executed after this one for specified Action Marker.
        /// </summary>
        /// <typeparam name="TActionMarker">Action interface</typeparam>
        public static PipelineConfigurator UseEventOnlySequenceMultiHandler<TActionMarker>(this PipelineConfigurator config)
            where TActionMarker : IEvent
        {
            return config.UseEventOnly<MultiHandlerSequenceExecutionEventMiddleware, TActionMarker>();
        }

        /// <summary>
        /// Pipeline running all handlers in sequence one by one. Not further pipeline will be executed after this one for specified Action Marker.
        /// </summary>
        /// <typeparam name="TActionMarker">Action interface</typeparam>
        public static PipelineConfigurator UseRequestOnlySequenceMultiHandler<TActionMarker>(this PipelineConfigurator config)
            where TActionMarker : IRequest
        {
            return config.UseRequestOnly<MultiHandlerSequenceExecutionRequestMiddleware, TActionMarker>();
        }
    }
}
