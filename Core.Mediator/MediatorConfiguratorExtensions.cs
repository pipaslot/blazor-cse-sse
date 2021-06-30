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
        public static PipelineConfigurator UseConcurrentMultiHandler<TActionMarker>(this PipelineConfigurator config)
            where TActionMarker : IActionMarker
        {
            return config.Use<MultiHandlerConcurrentExecutionMiddleware, TActionMarker>();
        }

        /// <summary>
        /// Pipeline running all handlers in sequence one by one. Not further pipeline will be executed after this one for specified Action Marker.
        /// </summary>
        /// <typeparam name="TActionMarker">Action interface</typeparam>
        public static PipelineConfigurator UseSequenceMultiHandler<TActionMarker>(this PipelineConfigurator config)
            where TActionMarker : IActionMarker
        {
            return config.Use<MultiHandlerSequenceExecutionMiddleware, TActionMarker>();
        }
    }
}
