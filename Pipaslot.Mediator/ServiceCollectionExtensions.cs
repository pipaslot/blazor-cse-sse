using Pipaslot.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Pipaslot.Mediator
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure handler sources and pipeline for handler processing
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IPipelineConfigurator AddMediator(this IServiceCollection services)
        {
            services.AddTransient<ServiceResolver>();
            services.AddTransient<IMediator, Mediator>();
            services.AddTransient<HandlerExistenceChecker>();
            var configurator = new PipelineConfigurator(services);
            services.AddSingleton(configurator);

            return configurator;
        }
    }
}
