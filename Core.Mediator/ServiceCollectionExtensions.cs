using System;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Mediator
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMediator(this IServiceCollection services)
        {
            services.AddTransient<IMediator, Mediator>();
        }

        public static IServiceCollection AddMediatorCommandPipeline(this IServiceCollection services, Type commandPipelineType)
        {
            services.AddScoped(typeof(ICommandPipeline<>), commandPipelineType);
            return services;
        }

        public static IServiceCollection AddMediatorQueryPipeline(this IServiceCollection services, Type queryPipelineType)
        {
            services.AddScoped(typeof(IQueryPipeline<,>), queryPipelineType);
            return services;
        }
    }
}
