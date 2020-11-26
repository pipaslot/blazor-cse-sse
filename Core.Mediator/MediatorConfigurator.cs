using System;
using System.Reflection;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Mediator
{
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
            _services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );
            return this;
        }
        /// <summary>
        /// Register pipelines in their order
        /// </summary>
        public MediatorConfigurator Use<TPipeline>() where TPipeline : IPipeline
        {
            return RegisterPipelines(typeof(TPipeline));
        }

        /// <summary>
        /// Register pipelines in their order with restricted request type implementation
        /// <typeparam name="TMarker">Only requests implementing TMarker class or interface will be processed by this pipeline</typeparam>
        /// </summary>
        public MediatorConfigurator Use<TPipeline, TMarker>() where TPipeline : IPipeline
        {
            return RegisterPipelines(typeof(TPipeline), typeof(TMarker));
        }

        private MediatorConfigurator RegisterPipelines(Type pipeline, Type? markerType = null)
        {
            _services.AddSingleton(new PipelineDefinition(pipeline, markerType));
            _services.AddScoped(pipeline);

            return this;
        }
    }
}
