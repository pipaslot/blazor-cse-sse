using System;
using Core.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Mediator
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMediator(this IServiceCollection services, Action<MediatorConfiguration> optionSetup)
        {
            services.AddTransient<IMediator, Mediator>();
            services.AddTransient<HandlerExistenceChecker>();
            var options = new MediatorConfiguration();
            optionSetup(options);

            // Automatically register all query handlers from project App.Server
            services.Scan(scan => scan
                .FromAssemblies(options.HandlerAssemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );
            foreach (var pipelineDefinition in options.Pipelines)
            {
                services.AddSingleton(pipelineDefinition);
                services.AddScoped(pipelineDefinition.PipelineType);
            }
        }
    }
}
