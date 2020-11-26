using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Mediator
{
    public class MediatorConfiguration
    {
        internal readonly List<Assembly> HandlerAssemblies = new List<Assembly>();
        internal readonly List<PipelineDefinition> Pipelines = new List<PipelineDefinition>();

        /// <summary>
        /// Will scan for types from the assembly of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type in which assembly that should be scanned.</typeparam>
        /// <example>AddHandlersFromAssemblyOf<ConfigQueryHandler>();</example>
        public MediatorConfiguration AddHandlersFromAssemblyOf<T>()
        {
            HandlerAssemblies.Add(typeof(T).Assembly);
            return this;
        }

        /// <summary>
        /// Scan assemblies for handler types
        /// </summary>
        /// <example>AddHandlersFromAssemblyOf(typeof(ConfigQueryHandler).Assembly);</example>
        public MediatorConfiguration AddHandlersFromAssembly(params Assembly[] assemblies)
        {
            HandlerAssemblies.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Register pipelines in their order
        /// </summary>
        /// <example>AddQueryPipeline(typeof(LoggingQueryPipeline<,>));</example>
        public MediatorConfiguration AddPipeline(params Type[] pipelines)
        {
            Pipelines.AddRange(pipelines.Select(p=>new PipelineDefinition(p)));
            return this;
        }
        /// <summary>
        /// Register pipelines in their order
        /// </summary>
        /// <example>AddQueryPipeline(typeof(LoggingQueryPipeline<,>));</example>
        /// <typeparam name="TMarker">Type which has t obe implemented by request for which the pipeline will eb applied for</typeparam>
        public MediatorConfiguration AddPipeline<TMarker>(params Type[] pipelines)
        {
            Pipelines.AddRange(pipelines.Select(p=>new PipelineDefinition(p, typeof(TMarker))));
            return this;
        }
    }
}
