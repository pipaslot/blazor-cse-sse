using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Mediator
{
    public class MediatorConfiguration
    {
        internal readonly List<Assembly> HandlerAssemblies = new List<Assembly>();
        internal readonly List<Type> CommandPipelines = new List<Type>();
        internal readonly List<Type> QueryPipelines = new List<Type>();

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
        /// <example>AddCommandPipeline(typeof(LoggingCommandPipeline<>));</example>
        public MediatorConfiguration AddCommandPipeline(params Type[] pipelines)
        {
            CommandPipelines.AddRange(pipelines);
            return this;
        }

        /// <summary>
        /// Register pipelines in their order
        /// </summary>
        /// <example>AddQueryPipeline(typeof(LoggingQueryPipeline<,>));</example>
        public MediatorConfiguration AddQueryPipeline(params Type[] pipelines)
        {
            QueryPipelines.AddRange(pipelines);
            return this;
        }
    }
}
