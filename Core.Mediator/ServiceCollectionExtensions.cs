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
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );

            // Automatically register all command handlers from project App.Server
            services.Scan(scan => scan
                .FromAssemblies(options.HandlerAssemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );
            foreach (var pipelineType in options.Pipelines)
            {
                services.AddScoped(typeof(IPipeline<,>), pipelineType);
            }
        }
    }
}
