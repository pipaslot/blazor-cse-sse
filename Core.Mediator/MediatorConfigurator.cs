using System;
using System.Linq;
using System.Reflection;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Mediator
{
    /// <summary>
    /// Scans assemblies for Handlers and specify pipelines by their order
    /// </summary>
    public class MediatorConfigurator
    {
        private readonly IServiceCollection _services;

        public MediatorConfigurator(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// Will scan for types from the assembly of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type in which assembly that should be scanned.</typeparam>
        /// <example>AddHandlersFromAssemblyOf<ConfigQueryHandler>();</example>
        public MediatorConfigurator AddHandlersFromAssemblyOf<T>()
        {
            return AddHandlersFromAssembly(typeof(T).Assembly);
        }

        /// <summary>
        /// Scan assemblies for handler types
        /// </summary>
        public MediatorConfigurator AddHandlersFromAssembly(params Assembly[] assemblies)
        {
            var handlerTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(IEventHandler<>)
            };
            var types = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && handlerTypes.Contains(i.GetGenericTypeDefinition())))
                .Select(t => new
                {
                    Type = t,
                    Interfaces = t.GetInterfaces()
                        .Where(i => i.IsGenericType && handlerTypes.Contains(i.GetGenericTypeDefinition()))
                });
            foreach (var pair in types)
            {
                foreach (var iface in pair.Interfaces)
                {
                    _services.AddTransient(iface, pair.Type);
                }
            }
            return this;
        }
        /// <summary>
        /// Register pipeline for all actions
        /// </summary>
        public MediatorConfigurator Use<TPipeline>()
            where TPipeline : IRequestPipeline, IEventPipeline
        {
            return RegisterPipelines(typeof(TPipeline));
        }

        /// <summary>
        /// Register pipeline for action classes implementing marker type only
        /// </summary>
        /// <typeparam name="TActionMarker">Action interface</typeparam>
        public MediatorConfigurator UseRequestOnly<TPipeline, TActionMarker>()
            where TPipeline : IRequestPipeline
            where TActionMarker : IRequest
        {
            return RegisterPipelines(typeof(TPipeline), typeof(TActionMarker));
        }
        /// <summary>
        /// Register pipeline for action classes implementing marker type only
        /// </summary>
        /// <typeparam name="TActionMarker">Action interface</typeparam>
        public MediatorConfigurator UseEventOnly<TPipeline, TActionMarker>()
            where TPipeline : IEventPipeline
            where TActionMarker : IEvent
        {
            return RegisterPipelines(typeof(TPipeline), typeof(TActionMarker));
        }

        private MediatorConfigurator RegisterPipelines(Type pipeline, Type? markerType = null)
        {
            _services.AddSingleton(new PipelineDefinition(pipeline, markerType));
            _services.AddScoped(pipeline);

            return this;
        }
    }
}
