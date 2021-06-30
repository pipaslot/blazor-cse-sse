using Core.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Mediator
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure handler sources and pipeline for handler processing
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static PipelineConfigurator AddMediator(this IServiceCollection services)
        {
            services.AddTransient<ServiceResolver>();
            services.AddTransient<IMediator, Mediator>();
            services.AddTransient<HandlerExistenceChecker>();
            var configurator = new PipelineConfigurator(services);

            return configurator;
        }
    }
}
